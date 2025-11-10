using Microsoft.ML;
using Microsoft.ML.Data;
using Mottu.FrotaApi.ML;

namespace Mottu.FrotaApi.Services
{
    public class DisponibilidadeTrainer
    {
        private readonly string _modelPath = "DisponibilidadeModel.zip";
        private readonly MLContext _mlContext;

        public DisponibilidadeTrainer()
        {
            _mlContext = new MLContext(seed: 123);
        }

        public string ModelPath => _modelPath;

        public bool ModelExists() => File.Exists(_modelPath);

        public void TreinarESalvarModelo(bool overwrite = false)
        {
            if (ModelExists() && !overwrite) return;

            // 1) Gerar dataset sintético (maior para estabilidade)
            var rnd = new Random(123);
            var dados = new List<DisponibilidadeInput>();

            // Regra hipotética: se em manutenção -> indisponível.
            // Se dias desde última entrega == 0 e km alto, mais propenso a estar indisponível.
            for (int i = 0; i < 2000; i++)
            {
                float km = (float)(rnd.NextDouble() * 50000); // 0 .. 50k
                float dias = rnd.Next(0, 10); // 0 .. 9 dias
                // 10% das motos em manutenção aleatoriamente
                float emManut = rnd.NextDouble() < 0.10 ? 1f : 0f;

                dados.Add(new DisponibilidadeInput
                {
                    KmRodados = km,
                    DiasDesdeUltimaEntrega = dias,
                    EmManutencao = emManut
                });
            }

            // Criar label lógico a partir das regras (simulando observações históricas)
            // Vamos converter para IDataView com campo Label (bool)
            var labeled = dados.Select(d =>
            {
                // Heurística para gerar rótulo:
                // - se em manutencao -> indisponível (Label = false)
                // - se dias == 0 && km > 20000 -> mais provável indisponível
                // - caso contrário disponível
                bool disponivel;
                if (d.EmManutencao == 1f) disponivel = false;
                else if (d.DiasDesdeUltimaEntrega == 0 && d.KmRodados > 20000) disponivel = false;
                else if (d.KmRodados > 40000) disponivel = false;
                else disponivel = true;

                return new
                {
                    d.KmRodados,
                    d.DiasDesdeUltimaEntrega,
                    d.EmManutencao,
                    Label = disponivel // Label = true => disponível
                };
            }).ToList();

            var data = _mlContext.Data.LoadFromEnumerable(labeled);

            // 2) Dividir em treino/test
            var split = _mlContext.Data.TrainTestSplit(data, testFraction: 0.2, seed: 123);
            var trainData = split.TrainSet;
            var testData = split.TestSet;

            // 3) Pipeline: Features -> Normalize -> Trainer (SDCA logistic)
            var pipeline = _mlContext.Transforms.Concatenate("Features", nameof(DisponibilidadeInput.KmRodados),
                    nameof(DisponibilidadeInput.DiasDesdeUltimaEntrega), nameof(DisponibilidadeInput.EmManutencao))
                .Append(_mlContext.Transforms.NormalizeMinMax("Features"))
                .Append(_mlContext.BinaryClassification.Trainers.SdcaLogisticRegression(labelColumnName: "Label"));

            // 4) Treinar
            var model = pipeline.Fit(trainData);

            // 5) Avaliar
            var predictions = model.Transform(testData);
            var metrics = _mlContext.BinaryClassification.Evaluate(predictions, labelColumnName: "Label");

            Console.WriteLine("=== Disponibilidade Model Metrics ===");
            Console.WriteLine($"Accuracy: {metrics.Accuracy:P2}");
            Console.WriteLine($"AUC: {metrics.AreaUnderRocCurve:P2}");
            Console.WriteLine($"F1Score: {metrics.F1Score:P2}");

            // 6) Salvar
            _mlContext.Model.Save(model, trainData.Schema, _modelPath);
            Console.WriteLine($"Modelo salvo em '{_modelPath}'");
        }

        public DisponibilidadePrediction Prever(DisponibilidadeInput input)
        {
            if (!ModelExists()) TreinarESalvarModelo();

            ITransformer model;
            DataViewSchema schema;
            model = _mlContext.Model.Load(_modelPath, out schema);

            var engine = _mlContext.Model.CreatePredictionEngine<DisponibilidadeInput, DisponibilidadePrediction>(model);
            var pred = engine.Predict(input);
            return pred;
        }
    }
}
