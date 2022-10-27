
public static class Algorithms
{
    public static void Swap<T>(ref T firstElement, ref T secondElement)
    {
        (firstElement, secondElement) = (secondElement, firstElement);
    }
}
