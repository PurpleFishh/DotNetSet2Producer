namespace Generator.Random;

public static class Shapes
{
    public static Func<double,double> Tri(double mode01) =>
        u => u < mode01 ? Math.Sqrt(u*mode01) : 1 - Math.Sqrt((1-u)*(1-mode01));
    public static Func<double,double> Mid(double k=2.0) =>
        u => Math.Pow(Math.Abs(2*u-1), 1.0/k) * Math.Sign(u-0.5) * 0.5 + 0.5;
}