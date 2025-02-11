//using OpenSimplexNoise;
using System;

public static class Noise
{
    private static OpenSimplexNoise noiseGenerator;

    static Noise()
    {
        noiseGenerator = new OpenSimplexNoise(Settings.SEED);
    }

    public static double Noise2(double x, double y)
    {
        return noiseGenerator.Evaluate(x, y);
    }

    public static double Noise3(double x, double y, double z)
    {
        return noiseGenerator.Evaluate(x, y, z);
    }
}
