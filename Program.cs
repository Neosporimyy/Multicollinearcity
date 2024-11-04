using System;
using System.Linq;

namespace StatisticsTests
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Введите количество независимых переменных X: ");
            int numOfIndependentVariables = int.Parse(Console.ReadLine());

            Console.WriteLine("Введите значения зависимой переменной Y через запятую: ");
            double[] y = Console.ReadLine().Split(',').Select(double.Parse).ToArray();

            double[][] x = new double[y.Length][];
            for (int i = 0; i < y.Length; i++)
            {
                x[i] = new double[numOfIndependentVariables + 1]; 
                x[i][0] = 1.0; 

                Console.WriteLine($"Введите значения для наблюдения {i + 1} (разделите пробелами): ");
                double[] independentValues = Console.ReadLine().Split(' ').Select(double.Parse).ToArray();

                for (int j = 1; j <= numOfIndependentVariables; j++)
                {
                    x[i][j] = independentValues[j - 1];
                }
            }

            // Вывод регрессионной модели
            PrintRegressionModel(x, y);

            Console.WriteLine("Проверка на мультиколлинеарность: " + CheckMulticollinearity(x));
            Console.WriteLine("Проверка на гетероскедастичность: " + CheckHeteroscedasticity(y, x));
            Console.WriteLine("Проверка на автокорреляцию: " + CheckAutocorrelation(y));
        }

        // Вывод регрессионной модели
        static void PrintRegressionModel(double[][] x, double[] y)
        {
            Console.WriteLine("Регрессионная модель:");

            // Здесь простой подход для демонстрации модели (например, среднее значение)
            Console.Write("Y = ");
            Console.Write("a0 (константа) + ");
            for (int j = 1; j < x[0].Length; j++)
            {
                Console.Write($"a{j} * X{j} ");
                if (j < x[0].Length - 1)
                {
                    Console.Write("+ ");
                }
            }
            Console.WriteLine();
        }

        // Проверка на мультиколлинеарность
        static bool CheckMulticollinearity(double[][] x)
        {
            int n = x[0].Length;
            for (int j = 1; j < n; j++) // Начинаем с 1, чтобы пропустить константу
            {
                for (int k = j + 1; k < n; k++)
                {
                    double correlation = CalculateCorrelation(x.Select(row => row[j]).ToArray(), x.Select(row => row[k]).ToArray());
                    if (Math.Abs(correlation) > 0.9) // Порог 0.9 для корреляции
                    {
                        return true; // Найдена высокая корреляция
                    }
                }
            }
            return false;
        }

        // Простое вычисление корреляции
        static double CalculateCorrelation(double[] x, double[] y)
        {
            double avgX = x.Average();
            double avgY = y.Average();
            double sumXY = 0, sumX2 = 0, sumY2 = 0;

            for (int i = 0; i < x.Length; i++)
            {
                sumXY += (x[i] - avgX) * (y[i] - avgY);
                sumX2 += (x[i] - avgX) * (x[i] - avgX);
                sumY2 += (y[i] - avgY) * (y[i] - avgY);
            }

            return sumXY / Math.Sqrt(sumX2 * sumY2);
        }

        // Проверка на гетероскедастичность
        static bool CheckHeteroscedasticity(double[] y, double[][] x)
        {
            double[] residuals = y.Select((value, index) => value - Predict(x[index])).ToArray();
            double standardDeviation = Math.Sqrt(residuals.Select(r => r * r).Average());
            double threshold = 1.0; // Порог, можно настроить
            return standardDeviation > threshold; // Признак гетероскедастичности
        }

        // Простой предсказательный метод
        static double Predict(double[] x)
        {
            return x.Sum() / x.Length; // Простое среднее значение как предсказание
        }

        // Проверка на автокорреляцию
        static bool CheckAutocorrelation(double[] y)
        {
            double[] differences = new double[y.Length - 1];
            for (int i = 1; i < y.Length; i++)
            {
                differences[i - 1] = y[i] - y[i - 1];
            }
            double correlation = CalculateCorrelation(differences, differences); // Используем ту же функцию
            return Math.Abs(correlation) > 0.5; // Порог для автокорреляции
        }
    }
}

