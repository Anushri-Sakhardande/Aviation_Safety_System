using System;
using System.Collections.Generic;
using Accord.Statistics.Models.Regression.Linear;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.IO;
using System.Collections;
using System.Formats.Asn1;
using System.Linq;

namespace LinearRegressionExample
{
    class Model
    {
        static void GetVals(string[] args)
        {
            // Load data from CSV file
            var data = ReadDataFromCsv("Aviation_Safety_Clean.csv");

            // Extract features and labels
            double[][] features = data.Item1;
            int[] samya = data.Item2;

            double[] labels = samya.Select(x => (double)x).ToArray();


            // Train the linear regression model
            var regression = new MultipleLinearRegression(2); // 2 features: country and manufacturer
            var teacher = new Accord.Statistics.Models.Regression.Linear.OrdinaryLeastSquares();
            regression = teacher.Learn(features, labels);

            // Make predictions on the training data
            double[] predictions = regression.Transform(features);

            // Compute the mean squared error
            double mse = ComputeMeanSquaredError(labels, predictions);
            Console.WriteLine($"Mean Squared Error: {mse}");

            // Get user input for country and manufacturer
            Console.Write("Enter the country: ");
            string input_country = Console.ReadLine();
            int country = input_country.GetHashCode();

            Console.Write("Enter the manufacturer: ");
            string input_manufacturer = Console.ReadLine();
            int manufacturer = input_manufacturer.GetHashCode();

            // Predict the number of fatalitieso 
            double[] input = { country, manufacturer };
            double prediction = regression.Transform(input);

            Console.WriteLine($"Predicted number of fatalities: {prediction}");
        }

        static Tuple<double[][], int[]> ReadDataFromCsv(string filePath)
        {
            List<double[]> features = new List<double[]>();
            List<int> labels = new List<int>();

            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                while (csv.Read())
                {
                    double[] row = new double[2]; // Assuming there are 2 features: country and manufacturer

                    // Read manufacturer from the 10th column (index 9)
                    string manufacturerStr = csv.GetField<string>(9);
                    // Convert manufacturer string to some numerical value (e.g., hash code)
                    row[0] = manufacturerStr.GetHashCode();

                    // Read country from the 11th column (index 10)
                    string countryStr = csv.GetField<string>(10);
                    // Convert country string to some numerical value (e.g., hash code)
                    row[1] = countryStr.GetHashCode();

                    features.Add(row);

                    //int label = csv.GetField<int>(6); // 7th column (index 6) contains "Fatalities"
                    int label = csv.GetField<int>(6);
                    labels.Add(label);
                }
            }

            return new Tuple<double[][], int[]>(features.ToArray(), labels.ToArray());
        }

        static double ComputeMeanSquaredError(double[] labels, double[] predictions)
        {
            double sumSquaredErrors = 0;
            for (int i = 0; i < labels.Length; i++)
            {
                double error = labels[i] - predictions[i];
                sumSquaredErrors += error * error;
            }
            return sumSquaredErrors / labels.Length;
        }
    }
}