using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NetworkTest2.Data;
using NetworkTest2.Helper;
using NetworkTest2.Model;

namespace NetworkTest2
{
    class Program
    {
        private static Random _random = new Random();

        static void Main(string[] args)
        {
            Time.LogAction += Console.WriteLine;
            Time.LogAction += s => Debug.WriteLine(s);
            Time.StartLogAction += Console.WriteLine;
            Time.StartLogAction += s => Debug.WriteLine(s);

            if (!Directory.Exists(@".\Networks"))
                Directory.CreateDirectory(@".\Networks");

            List<GrayscaleImage> trainImages;
            using (Time.Capture("Load_Training_Images"))
            {
                trainImages = new List<GrayscaleImage>(ImageLoader.LoadFromFile("train-images.dat", "train-labels.dat"));
            }

            List<GrayscaleImage> testImages;
            using (Time.Capture("Load_Training_Images"))
            {
                testImages = new List<GrayscaleImage>(ImageLoader.LoadFromFile("test-images.dat", "test-labels.dat"));
            }

            NeuralNetwork network = CreateNetwork();

            var imageCount = 60000;
            var trainBatchSize = 5000;
            var trainrate = 2.95;
            
            var batches = trainImages.Shuffle().Split(trainBatchSize).Select(x => x.ToList()).ToList();

            var tasks = new List<Task<NeuralNetwork>>();
            

            using (Time.Capture("Batch_Train_Networks"))
            {
                var i = 0;
                foreach (var batch in batches)
                {
                    i++;
                    var index = i;
                    tasks.Add(Task.Run(() =>
                    {
                        var nw = CreateNetwork();
                        nw.TrainImageSet(batch, trainrate);
                        nw.Save(@".\Networks\BatchTrainedNetwork_" + index + ".net");
                        return nw;
                    }));
                }

                Task.WaitAll(tasks.ToArray());
            }


            var networks = tasks.Select(t => t.Result);

            using (Time.Capture("Test_All_Networks"))
            {
                var i = 0;
                foreach (var neuralNetwork in networks)
                {
                    var result = TestImages(neuralNetwork, testImages);
                    i++;
                    Console.WriteLine($"{i:000}.: "+result.ToString());
                }
            }


            //var trainImages = images.Take(imageCount);
            //using (Time.Capture("Network_Training"))
            //{
            //    TrainImageSet(network, trainImages, 2.95);
            //}

            //network.Save(@".\TrainedNetwork5.net");

           //network = NeuronalExtensions.Load(@".\TrainedNetwork5.net");

           // var testImageCount = 100;

           // var testImages = trainImages.Shuffle().Take(testImageCount);
           // using (Time.Capture("Network_Testing"))
           // {
           //     TestImages(network, testImages);
           // }


            Console.WriteLine("-- finish. Press Enter to exit.");
            Console.ReadLine();
        }

        static NetworkResult TestImages(NeuralNetwork network, IEnumerable<GrayscaleImage> images)
        {
            var count = images.Count();
            var correct = 0;
            var costSum = 0d;
            foreach (var grayscaleImage in images)
            {
                var testResult = TestImage(network, grayscaleImage);

                costSum += testResult.Cost;
                if (testResult.IsCorrect) correct++;
            }

            costSum /= images.Count();

            var result = new NetworkResult(count, correct, costSum);

            Console.WriteLine($"     Tests: {result.Count}");
            Console.WriteLine($"   Correct: {result.Correct}");
            Console.WriteLine($" Correct %: {result.CorrectPercent:0.###}%");
            Console.WriteLine($"      Cost: {costSum:0.########}");
            Console.WriteLine($"#############################");

            return result;
        }

        static TestResult TestImage(NeuralNetwork network, GrayscaleImage image)
        {
            //Console.Write("Test Image with Label '"+image.ImageLabel+"': ");
           
            network.FeedImage(image);
            network.FeedForward();
            
            var classified = network.Neurons[network.LayerCount-1][0].Output;
            var classifiedIndex = 0;
            for (int i = 1; i < network.Neurons[network.LayerCount-1].Length; i++)
            {
                var value = network.Neurons[network.LayerCount - 1][i].Output;
                if (value > classified)
                {
                    classified = value;
                    classifiedIndex = i;
                }
            }

            var isCorrect = classifiedIndex == image.ImageLabel;
            var cost = network.CalculateCost(classifiedIndex);

            //Console.Write(classifiedIndex + " ("+(isCorrect ? "Correct" : "Incorrect")+"). Cost: ");
            //Console.WriteLine(cost);
            //network.PrintLayer(network.LayerCount - 1);
            //Console.WriteLine("-----------------------");

            return new TestResult()
            {
                Cost = cost,
                IsCorrect = isCorrect
            };
        }

        static void TrainImageSet(NeuralNetwork network, IEnumerable<GrayscaleImage> images, double rate = 1.0)
        {
            using (Time.Capture("Train_" + images.Count() + "_Images"))
            {
                network.TrainImageSet(images, rate);
            }
        }

        static NeuralNetwork CreateNetwork()
        {
            NeuralNetwork network = new NeuralNetwork(4);
            network.SetLayerNeurons(0, 784, IntitialBias);
            network.SetLayerNeurons(1, 16, IntitialBias);
            network.SetLayerNeurons(2, 16, IntitialBias);
            network.SetLayerNeurons(3, 10, IntitialBias);
            network.SetupSynapses(InitialWeight);
            return network;
        }

        static NeuralNetwork LoadNetwork(string path)
        {
            return NeuronalExtensions.Load(path);
        }

        static double IntitialBias(int i)
        {
            //return 0;
            return _random.NextDouble() * 20.0 - 10.0;
        }

        static double InitialWeight(int i, int j, int k)
        {
            //return 1;
            return _random.NextDouble() * 2.0 - 1.0;
        }


        class NetworkResult
        {
            public int Count;
            public int Correct;
            public double CorrectPercent;
            public double Cost;

            public NetworkResult(int count, int correct, double cost)
            {
                Count = count;
                Correct = correct;
                Cost = cost;
                CorrectPercent = (double) correct / (double) count * 100.0;
            }
            public override string ToString()
            {
                return $"[{Correct}/{Count} ({CorrectPercent:0.###}%) | Cost: {Cost:0.########}]";
            }
        }

        class TestResult
        {
            public bool IsCorrect;
            public double Cost;
        }
    }
}
