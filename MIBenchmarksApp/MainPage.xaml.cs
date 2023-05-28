using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using MIBenchmarksApp.Methods;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.Maui;
using MIBenchmarksApp.models;
using LiveChartsCore.SkiaSharpView.SKCharts;
using SkiaSharp;
using ClosedXML;
using System.Reflection;
using static SkiaSharp.HarfBuzz.SKShaper;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Diagnostics;
using Item = MIBenchmarksApp.Methods.Item;
using DocumentFormat.OpenXml.Drawing;

namespace MIBenchmarksApp;

public partial class MainPage : ContentPage
{

    SKColor[] Colors { get; } = new SKColor[]
    {
    SKColors.Red,
    SKColors.Blue,
    SKColors.Green,
    SKColors.Yellow,
    SKColors.Purple,
    SKColors.Orange,
    SKColors.Pink,
    SKColors.Brown,
    SKColors.LightBlue,
    SKColors.LightGreen,
    SKColors.LightYellow,
    SKColors.LightPink,
    SKColors.SkyBlue,
    SKColors.ForestGreen,
    SKColors.Gold,
    SKColors.Magenta,
    SKColors.Maroon,
    SKColors.Navy,
    SKColors.Olive,
    SKColors.Silver,
    SKColors.Teal,
    SKColors.Violet,
    SKColors.Wheat,
    SKColors.Tan,
    SKColors.SlateBlue,
    SKColors.SeaGreen,
    SKColors.Salmon,
    SKColors.RosyBrown,
    SKColors.Plum,
    SKColors.PeachPuff
    };


    public MainPage()
    {
        InitializeComponent();



        //foreach (var results in rsResults)
        //{
        //    foreach (var result in results)
        //    {
        //        Console.WriteLine($"RS Min: {StatisticFunctions.Min(result)}, Max: {StatisticFunctions.Max(result)}, Mean: {StatisticFunctions.Mean(result)}, Median: {StatisticFunctions.Median(result)}, StdDev: {StatisticFunctions.StdDev(result)}");
        //    }
        //}
    }

    private async void _StartBtn_Clicked(object sender, EventArgs e)
    {
        _StartBtn.Text = "Generování spuštěno!";
        _Indicator.IsRunning = true;
        List<List<double>> avgs1 = await Task.Run(() => DoTheRS());
        List<List<double>> avgs2 = await Task.Run(() => DoTheSA());
        await Task.Run(() => GenerateComparisonGraphs(avgs1, avgs2));
        _Indicator.IsRunning = false;
        await App.Current.MainPage.DisplayAlert("V pořádku", "Generování dokončeno", "Rozumím");
        _StartBtn.Text = "Spustit generování";
    }

    private List<List<double>> DoTheSA()
    {
        int maxFES = 10000;
        int maxTemp = 1000;
        double minTemp = 0.01;
        double coolingDecr = 0.98;
        int d1 = 5;
        int d2 = 10;
        double step = 0.1;
        int metropolis = 10;

        SimulatedAnnealing sa1 = new SimulatedAnnealing(d1, maxFES, TestFunctions.FirstFunction, minTemp, maxTemp, coolingDecr, step, metropolis, -5, 5);
        SimulatedAnnealing sa2 = new SimulatedAnnealing(d2, maxFES, TestFunctions.FirstFunction, minTemp, maxTemp, coolingDecr, step, metropolis, -5, 5);
        SimulatedAnnealing sa3 = new SimulatedAnnealing(d1, maxFES, TestFunctions.SecondFunction, minTemp, maxTemp, coolingDecr, step, metropolis, -5, 5);
        SimulatedAnnealing sa4 = new SimulatedAnnealing(d2, maxFES, TestFunctions.SecondFunction, minTemp, maxTemp, coolingDecr, step, metropolis, -5, 5);
        SimulatedAnnealing sa5 = new SimulatedAnnealing(d1, maxFES, TestFunctions.Schweffel, minTemp, maxTemp, coolingDecr, step, metropolis, -500, 500);
        SimulatedAnnealing sa6 = new SimulatedAnnealing(d2, maxFES, TestFunctions.Schweffel, minTemp, maxTemp, coolingDecr, step, metropolis, -500, 500);

        List<SimulatedAnnealing> simulatedAnnealings = new()
        {
            sa1, sa2, sa3, sa4, sa5, sa6
        };

        List<SAResult> saResults = new();
        List<List<SAResult>> saCycleResults = new();

        foreach (var sa in simulatedAnnealings)
        {
            for (int i = 0; i < 30; i++)
            {
                saResults.Add(sa.Compute());
            }
            saCycleResults.Add(new(saResults));
            saResults.Clear();
        }

        CartesianChart chart = new CartesianChart();
        List<ISeries> series = new();
        List<double> avgResult = new();
        List<List<double>> avgResults = new();

        //EXCEL
        var wb = new XLWorkbook();

        var xAxis = new Axis
        {
            MaxLimit = 5000,
            MinLimit = 0
        };

        var yAxis = new Axis
        {
            MaxLimit = 100,
            MinLimit = 0,
            MinStep = 20
        };

        int j = 0;
        foreach (var results in saCycleResults)
        {
            foreach (var result in results)
            {
                series.Add(new LineSeries<double>
                {
                    Values = result.AllCosts,
                    Fill = null,
                    GeometrySize = 0,
                    LineSmoothness = 0,
                    Stroke = new SolidColorPaint(Colors[series.Count % Colors.Length]) { StrokeThickness = 2 },
                });
            }
            chart.Series = series;

            SKCartesianChart skChart = new SKCartesianChart(chart)
            {
                Width = 1200,
                Height = 900
            };

            string folderPath = AppDomain.CurrentDomain.BaseDirectory;
            DirectoryInfo dirInfo = new DirectoryInfo(folderPath);
            dirInfo = dirInfo.Parent.Parent.Parent.Parent.Parent;
            folderPath = System.IO.Path.Combine(dirInfo.FullName, "Images");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string label = ChooseLabel(j);

            skChart.SaveImage(System.IO.Path.Combine(folderPath, $"SAchart{label}.png"));
            series.Clear();

            //prumerny graf
            for (int y = 0; y < results.First().AllCosts.Count; y++)
            {
                avgResult.Add(results.Select(x => x.AllCosts[y]).Average());
            }

            series.Add(new LineSeries<double>
            {
                Values = avgResult,
                Fill = null,
                GeometrySize = 0,
                LineSmoothness = 0,
                Stroke = new SolidColorPaint(Colors[series.Count % Colors.Length]) { StrokeThickness = 2 },
            });
            chart.Series = series;

            skChart.SaveImage(System.IO.Path.Combine(folderPath, $"SAchart{label}_AVG.png"));

            //ulozeni bokem, pro navratovou hodnotu --> 6 prumeru
            avgResults.Add(new(avgResult));

            //potreba clearovat
            series.Clear();
            avgResult.Clear();

            //vypis statistik do excelu

            var workSheet = wb.Worksheets.Add("SA_" + ChooseLabel(j));
            workSheet.Cell("A1").Value = "Iterace";
            workSheet.Cell("B1").Value = "Cena";
            workSheet.Cell("C1").Value = "Vstupy";



            for (int i = 0; i < results.Count; i++)
            {
                workSheet.Cell("A" + (i + 2).ToString()).Value = i;
                workSheet.Cell("B" + (i + 2).ToString()).Value = results[i].BestCost;

                char column = 'C';
                foreach (var arg in results[i].BestArgs)
                {
                    workSheet.Cell(column + (i + 2).ToString()).Value = arg.ToString();
                    column++;
                }
            }

            int columnNumber = results.Count + 5;
            workSheet.Cell("A" + (columnNumber).ToString()).Value = "Mean";
            workSheet.Cell("A" + (columnNumber + 1).ToString()).Value = "Median";
            workSheet.Cell("A" + (columnNumber + 2).ToString()).Value = "Min";
            workSheet.Cell("A" + (columnNumber + 3).ToString()).Value = "Max";
            workSheet.Cell("A" + (columnNumber + 4).ToString()).Value = "StdDev";

            workSheet.Cell("B" + (columnNumber).ToString()).FormulaA1 = "=AVERAGEA(B2:B31)";
            workSheet.Cell("B" + (columnNumber + 1).ToString()).FormulaA1 = "=MEDIAN(B2:B31)";
            workSheet.Cell("B" + (columnNumber + 2).ToString()).FormulaA1 = "=MIN(B2:B31)";
            workSheet.Cell("B" + (columnNumber + 3).ToString()).FormulaA1 = "=MAX(B2:B31)";
            workSheet.Cell("B" + (columnNumber + 4).ToString()).FormulaA1 = "=STDEVA(B2:B31)";

            j++;
        }

        saCycleResults.Clear();

        wb.SaveAs(System.IO.Path.Combine(GetFolderPath(), $"SA.xlsx"));
        return avgResults;
    }

    private List<List<double>> DoTheRS()
    {
        int maxFES = 10000;
        int d1 = 5;
        int d2 = 10;

        RandomSearch rs1 = new RandomSearch(d1, maxFES, TestFunctions.FirstFunction, -5, 5);
        RandomSearch rs2 = new RandomSearch(d2, maxFES, TestFunctions.FirstFunction, -5, 5);
        RandomSearch rs3 = new RandomSearch(d1, maxFES, TestFunctions.SecondFunction, -5, 5);
        RandomSearch rs4 = new RandomSearch(d2, maxFES, TestFunctions.SecondFunction, -5, 5);
        RandomSearch rs5 = new RandomSearch(d1, maxFES, TestFunctions.Schweffel, -500, 500);
        RandomSearch rs6 = new RandomSearch(d2, maxFES, TestFunctions.Schweffel, -500, 500);

        List<RandomSearch> randomSearches = new()
        {
            rs1 , rs2 , rs3 , rs4 , rs5 , rs6
        };


        List<RSResult> rsResults = new();
        List<List<RSResult>> rsCycleResults = new();
        List<double> avgResult = new();
        List<List<double>> avgResults = new();

        foreach (var rs in randomSearches)
        {
            for (int i = 0; i < 30; i++)
            {
                rsResults.Add(rs.Compute());
            }
            rsCycleResults.Add(new(rsResults));
            rsResults.Clear();
        }

        CartesianChart chart = new CartesianChart();
        List<ISeries> series = new();

        //EXCEL
        var wb = new XLWorkbook();

        //cesta k slozce
        string folderPath = AppDomain.CurrentDomain.BaseDirectory;
        DirectoryInfo dirInfo = new DirectoryInfo(folderPath);
        dirInfo = dirInfo.Parent.Parent.Parent.Parent.Parent;
        folderPath = System.IO.Path.Combine(dirInfo.FullName, "Images");

        int j = 0;
        foreach (var results in rsCycleResults)
        {
            foreach (var result in results)
            {
                series.Add(new LineSeries<double>
                {
                    Values = result.AllFitness,
                    Fill = null,
                    GeometrySize = 0,
                    LineSmoothness = 0,
                    Stroke = new SolidColorPaint(Colors[series.Count % Colors.Length]) { StrokeThickness = 2 },
                });
            }

            chart.Series = series;
            SKCartesianChart skChart = new SKCartesianChart(chart)
            {
                Width = 1200,
                Height = 900
            };



            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string label = ChooseLabel(j);

            //ulozeni do projektove slozky
            skChart.SaveImage(System.IO.Path.Combine(folderPath, $"RSchart{label}.png"));
            series.Clear();

            //prumerny graf
            for (int y = 0; y < results.First().AllFitness.Count; y++)
            {
                avgResult.Add(results.Select(x => x.AllFitness[y]).Average());
            }

            series.Add(new LineSeries<double>
            {
                Values = avgResult,
                Fill = null,
                GeometrySize = 0,
                LineSmoothness = 0,
                Stroke = new SolidColorPaint(Colors[series.Count % Colors.Length]) { StrokeThickness = 2 },
            });
            chart.Series = series;

            skChart.SaveImage(System.IO.Path.Combine(folderPath, $"RSchart{label}_AVG.png"));

            //ulozeni bokem, pro navratovou hodnotu --> 6 prumeru
            avgResults.Add(new(avgResult));

            //potreba clearovat
            series.Clear();
            avgResult.Clear();

            //vypis statistik do excelu

            var workSheet = wb.Worksheets.Add("RS_" + ChooseLabel(j));
            workSheet.Cell("A1").Value = "Iterace";
            workSheet.Cell("B1").Value = "Cena";
            workSheet.Cell("C1").Value = "Vstupy";



            for (int i = 0; i < results.Count; i++)
            {
                workSheet.Cell("A" + (i + 2).ToString()).Value = i;
                workSheet.Cell("B" + (i + 2).ToString()).Value = results[i].BestFitness;

                char column = 'C';
                foreach (var arg in results[i].BestArgs)
                {
                    workSheet.Cell(column + (i + 2).ToString()).Value = arg.ToString();
                    column++;
                }
            }

            int columnNumber = results.Count + 5;
            workSheet.Cell("A" + (columnNumber).ToString()).Value = "Mean";
            workSheet.Cell("A" + (columnNumber + 1).ToString()).Value = "Median";
            workSheet.Cell("A" + (columnNumber + 2).ToString()).Value = "Min";
            workSheet.Cell("A" + (columnNumber + 3).ToString()).Value = "Max";
            workSheet.Cell("A" + (columnNumber + 4).ToString()).Value = "StdDev";

            workSheet.Cell("B" + (columnNumber).ToString()).FormulaA1 = "=AVERAGEA(B2:B31)";
            workSheet.Cell("B" + (columnNumber + 1).ToString()).FormulaA1 = "=MEDIAN(B2:B31)";
            workSheet.Cell("B" + (columnNumber + 2).ToString()).FormulaA1 = "=MIN(B2:B31)";
            workSheet.Cell("B" + (columnNumber + 3).ToString()).FormulaA1 = "=MAX(B2:B31)";
            workSheet.Cell("B" + (columnNumber + 4).ToString()).FormulaA1 = "=STDEVA(B2:B31)";


            j++;
        }

        rsCycleResults.Clear();

        wb.SaveAs(System.IO.Path.Combine(GetFolderPath(), $"RS.xlsx"));

        return avgResults;

    }

    private string GetFolderPath()
    {
        string folderPath = AppDomain.CurrentDomain.BaseDirectory;
        DirectoryInfo dirInfo = new DirectoryInfo(folderPath);
        dirInfo = dirInfo.Parent.Parent.Parent.Parent.Parent;
        return System.IO.Path.Combine(dirInfo.FullName, "Images");
    }

    private string ChooseLabel(int j)
    {
        string label = string.Empty;
        switch (j)
        {
            case 0:
                label = "DJ1DIM5";
                break;
            case 1:
                label = "DJ1DIM10";
                break;
            case 2:
                label = "DJ2DIM5";
                break;
            case 3:
                label = "DJ2DIM10";
                break;
            case 4:
                label = "SCHDIM5";
                break;
            case 5:
                label = "SCHDIM10";
                break;
            default:
                break;
        }
        return label;
    }

    private void GenerateComparisonGraphs(List<List<double>> avgs1, List<List<double>> avgs2)
    {

        if (!(avgs1.Count == 6 && avgs2.Count == 6))
        {
            return;
        }
        List<ISeries> series = new();
        for (int i = 0; i < avgs1.Count; i++)
        {
            series.Add(new LineSeries<double>
            {
                Values = avgs1.ElementAt(i),
                Fill = null,
                GeometrySize = 0,
                LineSmoothness = 0,
                Stroke = new SolidColorPaint(SKColors.Red) { StrokeThickness = 2 },
                Name = "Simulated annealing"

            });
            series.Add(new LineSeries<double>
            {
                Values = avgs2.ElementAt(i),
                Fill = null,
                GeometrySize = 0,
                LineSmoothness = 0,
                Stroke = new SolidColorPaint(SKColors.Blue) { StrokeThickness = 2 },
                Name = "Random search"
            });
            CartesianChart chart = new();
            chart.Series = series;
            chart.LegendPosition = LiveChartsCore.Measure.LegendPosition.Top;
            SKCartesianChart skChart = new SKCartesianChart(chart)
            {
                Width = 1200,
                Height = 900
            };

            string folderPath = GetFolderPath();
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string label = ChooseLabel(i);
            skChart.SaveImage(System.IO.Path.Combine(folderPath, $"Comparison_{label}.png"), quality: 100);

            series.Clear();
        }
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        _Indicator2.IsRunning = true;
        await Task.Run(() => DoTheKPBrute());
        await Task.Run(() => DoTheKPSA());
        _Indicator2.IsRunning = false;
    }

    private void DoTheKPBrute()
    {
        Random rnd = new Random();


        // generujeme seznam předmětů
        List<Item> items = new();
        for (int i = 0; i < 25; i++)
        {
            items.Add(new Item
            {
                Id = i,
                Weight = rnd.Next(1, 50),
                Value = rnd.Next(1, 50)
            });
        }

        int knapsackCapacity = 200;

        Knapsack kp = new(items, knapsackCapacity);

        var stopwatch = Stopwatch.StartNew();
        var bestSolution = kp.ComputeBruteForce();
        stopwatch.Stop();

        int valueSum = bestSolution.Sum(x => x.Value);
        int weightSum = bestSolution.Sum(x => x.Weight);
        Console.WriteLine($"Best solution value: {valueSum}");
        Console.WriteLine($"Best solution weight: {weightSum}");
        Console.WriteLine($"Time taken: {stopwatch.ElapsedMilliseconds}ms");
    }

    private void DoTheKPSA()
    {
        Random rnd = new Random();
        List<Item> items = new();
        for (int i = 0; i < 25; i++)
        {
            items.Add(new Item
            {
                Id = i,
                Weight = rnd.Next(1, 50),
                Value = rnd.Next(1, 50)
            });
        }

        int knapsackCapacity = 200;

        Knapsack kp = new(items, knapsackCapacity);

        var stopwatch = Stopwatch.StartNew();
        var bestSolution = kp.ComputeSA();

        List<Item> acceptedItems = new();
        for (int i = 0; i < bestSolution.BestArgs.Count; i++)
        {
            if (bestSolution.BestArgs.ElementAt(i) == 1)
            {
                acceptedItems.Add(new Item()
                {
                    Id = items[i].Id,
                    Weight = items[i].Weight,
                    Value = items[i].Value
                });
            }
        }

        stopwatch.Stop();

        int valueSum = acceptedItems.Sum(x => x.Value);
        int weightSum = acceptedItems.Sum(x => x.Weight);
        Console.WriteLine($"Best solution value: {valueSum}");
        Console.WriteLine($"Best solution weight: {weightSum}");
        Console.WriteLine($"Time taken: {stopwatch.ElapsedMilliseconds}ms");
    }
}


