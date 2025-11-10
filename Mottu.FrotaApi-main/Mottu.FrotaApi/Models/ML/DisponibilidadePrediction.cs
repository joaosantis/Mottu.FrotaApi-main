using Microsoft.ML.Data;

namespace Mottu.FrotaApi.ML
{
    // Classe interna para receber a predição do ML.NET
    public class DisponibilidadePrediction
    {
        [ColumnName("PredictedLabel")]
        public bool PredictedLabel { get; set; }

        public float Score { get; set; }       
        public float Probability { get; set; } 
    }
}
