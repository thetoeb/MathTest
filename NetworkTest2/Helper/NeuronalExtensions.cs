using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NetworkTest2.Data;
using NetworkTest2.Model;

namespace NetworkTest2.Helper
{
    public static class NeuronalExtensions
    {
        public static void FeedImage(this NeuralNetwork network, GrayscaleImage image)
        {
            for (var row = 0; row < image.Height; row++)
            {
                for (var column = 0; column < image.Width; column++)
                {
                    var neuronIndex = row * image.Width + column;
                    var value = (double) image.ImageData[column, row] / 255.0;
                    network.Neurons[0][neuronIndex].Input = value;
                }
            }
        }

        public static void TrainImageSet(this NeuralNetwork network, IEnumerable<GrayscaleImage> images, double rate = 1.0)
        {
            var count = images.Count();
            var i = 0;
            foreach (var grayscaleImage in images)
            {
                network.FeedImage(grayscaleImage);
                network.FeedForward();

                var desiredOutput = network.GetDesiredOutput(grayscaleImage.ImageLabel);

                network.PropagateBackwards(desiredOutput, rate);

                var perc = (double) i / count * 100.0;
                if (Math.Abs(perc % 10) < 0.0000001) Console.WriteLine($"{perc}%");

                i++;
            }
        }

        public static void Save(this NeuralNetwork network, string path)
        {
            using (var fileStream = File.Open(path, FileMode.OpenOrCreate, FileAccess.Write))
            {
                using (var w = new BinaryWriter(fileStream))
                {
                    w.Write(network.LayerCount);
                    w.Write(network.Neurons[0].Length);
                    w.Write(network.Neurons[network.LayerCount-1].Length);

                    for (int layer = 0; layer < network.LayerCount; layer++)
                        w.Write(network.Neurons[layer].Length);
                    
                    for (var layer = 0; layer < network.LayerCount; layer++)
                    {
                        for (var neuron = 0; neuron < network.Neurons[layer].Length; neuron++)
                        {
                            w.Write(0);
                            w.Write(network.Neurons[layer][neuron].Bias);

                            if (layer < network.LayerCount - 1)
                            {
                                for (var connectingNeuron = 0; connectingNeuron < network.Neurons[layer + 1].Length; connectingNeuron++)
                                    w.Write(network.Synapses[layer][neuron][connectingNeuron].Weight);
                            }
                        }
                    }
                }
            }
        }

        public static NeuralNetwork Load(string path)
        {
           
            using (var fileStream = File.OpenRead(path))
            {
                using (var r = new BinaryReader(fileStream))
                {
                    var layerCount = r.ReadInt32();
                    var inputNeurons = r.ReadInt32();
                    var outputNeurons = r.ReadInt32();

                    var network = new NeuralNetwork(layerCount);
                    var neuronCount = new int[layerCount];
                    for (var layer = 0; layer < layerCount; layer++)
                    {
                        neuronCount[layer] = r.ReadInt32();
                        network.SetLayerNeurons(layer, neuronCount[layer]);
                    }
                    network.SetupSynapses();

                    for (var layer = 0; layer < layerCount; layer++)
                    {
                        for (var neuron = 0; neuron < neuronCount[layer]; neuron++)
                        {
                            var neuronType = r.ReadInt32();
                            network.Neurons[layer][neuron].Bias = r.ReadDouble();

                            if (layer < layerCount - 1)
                            {
                                for (var connectingNeuron = 0; connectingNeuron < neuronCount[layer + 1]; connectingNeuron++)
                                {
                                    network.Synapses[layer][neuron][connectingNeuron].Weight = r.ReadDouble();
                                }
                            }
                        }
                    }

                    return network;
                }
            }
        }

        public static void PrintLayer(this NeuralNetwork network, int layer)
        {
            for (var i = 0; i < network.Neurons[layer].Length; i++)
            {
                Console.WriteLine($"{i:00}.: {network.Neurons[layer][i].Output:0.######}");
            }
        }

        public static double CalculateCost(this NeuralNetwork network, int classification)
        {
            var desired = network.GetDesiredOutput(classification);
            return network.CalculateCost(desired);
        }

        public static double[] GetDesiredOutput(this NeuralNetwork network, int index)
        {
            var desired = new double[network.Neurons[network.LayerCount - 1].Length];
            desired[index] = 1.0;
            return desired;
        }
    }
}
