namespace Mottu.FrotaApi.ML
{
    public class DisponibilidadeInput
    {
        // Features (float preferred for ML.NET)
        public float KmRodados { get; set; }
        public float DiasDesdeUltimaEntrega { get; set; }
        public float EmManutencao { get; set; } // 1 = sim, 0 = não
    }
}
