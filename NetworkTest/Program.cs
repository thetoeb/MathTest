using System;
using System.Diagnostics;
using System.Linq;
using NetworkLib;
using NetworkLib.Model;
using NetworkLib.Model.Neurons;
using NetworkLib.Model.Synapses;

namespace NetworkTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //BasicTest();
            SimpleTest();
        }

        static double Sigmoid(double value)
        {
            double k = Math.Exp(value);
            return k / (1d + k);
        }

        static void SimpleTest()
        {
            var neuralNetwork = new NeuronalNetwork();
            var network = new SimpleNetwork(neuralNetwork, Sigmoid, Sigmoid);

            var layers = new int[5];
            layers[0] = 2;
            layers[1] = 3;
            layers[2] = 5;
            layers[3] = 4;
            layers[4] = 3;

            network.Init(layers);

            Console.WriteLine($"Created Network with {layers[0]} input neurons and {layers[layers.Length-1]} output neurons.");

            var consoleInput = string.Empty;
            var stopWatch = new Stopwatch();

            do
            {
                Console.WriteLine("Please enter all input values:");
                for (var i = 0; i < layers[0]; i++)
                {
                    Console.Write($"{i}. > ");
                    consoleInput = Console.ReadLine();
                    if (double.TryParse(consoleInput, out var value))
                    {
                        network.SetInputValue(i, value);
                    }
                }

                Console.WriteLine("Process..");
                stopWatch.Reset();
                stopWatch.Start();

                network.Process();

                stopWatch.Stop();
                Console.WriteLine($"Finished! Time: {stopWatch.ElapsedTicks} Ticks ({stopWatch.ElapsedMilliseconds} ms)");

                Console.WriteLine("Output values:");
                for (var i = 0; i < layers[layers.Length - 1]; i++)
                {
                    Console.WriteLine($"{i}.: {network.GetOutputValue(i)}");
                }

                Console.Write("Enter anything to exit.");
                consoleInput = Console.ReadLine();
            } while (consoleInput == string.Empty);
        }

        static void BasicTest()
        {
            var inputNeuron = new InputNeuron();
            var calcNeuron1 = new FunctionNeuronProcess(ds => ds * 2);
            var calcNeuron2 = new FunctionNeuronProcess(ds => ds * 2);
            var outputNeuron = new OutputNeuron(ds => ds * 2);

            var network = new NeuronalNetwork();
            network.AddNeuron(inputNeuron);
            network.AddNeuron(calcNeuron1);
            network.AddNeuron(calcNeuron2);
            network.AddNeuron(outputNeuron);

            var synapse11 = network.CreateSynapse(inputNeuron, calcNeuron1);
            var synapse12 = network.CreateSynapse(inputNeuron, calcNeuron2);
            var synapse21 = network.CreateSynapse(calcNeuron1, outputNeuron);
            var synapse22 = network.CreateSynapse(calcNeuron2, outputNeuron);

            synapse11.Weight = 2;
            synapse12.Weight = 2;
            synapse21.Weight = 2;
            synapse22.Weight = 2;

            Console.WriteLine("Write any Value");

            var input = string.Empty;
            do
            {
                network.Reset();

                Console.Write("> ");
                input = Console.ReadLine();

                if (double.TryParse(input, out double value))
                {
                    inputNeuron.Value = value;
                    Console.Write("Calculating..");
                    network.FeedForward();
                    Console.WriteLine("Finished. Result: " + outputNeuron.Value);
                }

            } while (input != string.Empty);
        }
    }
}
