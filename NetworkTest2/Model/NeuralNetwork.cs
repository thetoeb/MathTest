using System;
using System.Collections.Generic;
using System.Text;
using NetworkTest2.Helper;

namespace NetworkTest2.Model
{
    public class NeuralNetwork
    {
        public int LayerCount;
        public Neuron[][] Neurons;
        public Synapse[][][] Synapses;

        public NeuralNetwork(int layer)
        {
            LayerCount = layer;
            Neurons = new Neuron[LayerCount][];
            Synapses = new Synapse[LayerCount - 1][][];
        }

        public void SetLayerNeurons(int layer, int neurons, Func<int, double> neuronInitialBias = null)
        {
            if (neuronInitialBias == null)
                neuronInitialBias = i => 0;

            Neurons[layer] = new Neuron[neurons];
            for (var i = 0; i < neurons; i++)
            {
                Neurons[layer][i] = new Neuron();
                Neurons[layer][i].Bias = neuronInitialBias(i);
            }
        }

        public void SetupSynapses(Func<int,int,int,double> synapseInitialWeight = null)
        {
            if (synapseInitialWeight == null)
                synapseInitialWeight = (i, j, k) => 1;

            for (var layer = 0; layer < LayerCount - 1; layer++)
            {
                var layerNeuronCount = Neurons[layer].Length;
                Synapses[layer] = new Synapse[layerNeuronCount][];

                for (var layerNeuron = 0; layerNeuron < layerNeuronCount; layerNeuron++)
                {
                    var nextLayerNeuronCount = Neurons[layer + 1].Length;
                    Synapses[layer][layerNeuron] = new Synapse[nextLayerNeuronCount];

                    for (int nextLayerNeuron = 0; nextLayerNeuron < nextLayerNeuronCount; nextLayerNeuron++)
                    {
                        Synapses[layer][layerNeuron][nextLayerNeuron] = new Synapse();
                        Synapses[layer][layerNeuron][nextLayerNeuron].Weight = synapseInitialWeight(layer, layerNeuron, nextLayerNeuron);
                    }
                }
            }
        }

        public void FeedForward()
        {
            for (var layer = 0; layer < LayerCount; layer++)
            {
                var neurons = Neurons[layer].Length;
                for (var layerNeuron = 0; layerNeuron < neurons; layerNeuron++)
                {
                    // Only after the first layer
                    if (layer > 0)
                    {
                        // Add up all Synapse-Outputs from the last Layer for this Neuron
                        Neurons[layer][layerNeuron].Input = 0;
                        for (var i = 0; i < Neurons[layer-1].Length; i++)
                            Neurons[layer][layerNeuron].Input += Synapses[layer - 1][i][layerNeuron].Output;

                        // Process the summed value
                        Neurons[layer][layerNeuron].Process();
                    }
                    else
                    {
                        // Set the Neurons Input to the Output in the first Layer
                        Neurons[layer][layerNeuron].Output = Neurons[layer][layerNeuron].Input;
                    }
                    
                    // Apply output value to synapses
                    if (layer < LayerCount - 1)
                    {
                        for (var i = 0; i < Neurons[layer + 1].Length; i++)
                        {
                            Synapses[layer][layerNeuron][i].Input = Neurons[layer][layerNeuron].Output;
                            Synapses[layer][layerNeuron][i].Process();
                        }
                    }
                }
            }
        }

        public void PropagateBackwards(double[] expected, double rate = 1.0)
        {
            var layerLength = Neurons[LayerCount - 1].Length;
            if (expected.Length != layerLength) throw new ArgumentException();

            for (var i = 0; i < layerLength; i++)
            {
                var neuron = Neurons[LayerCount - 1][i];
                neuron.Error = neuron.DerivateProcess(neuron.Output) * (expected[i] - neuron.Output);
            }

            // Go from second last layer to first one
            for (var layer = LayerCount - 2; layer >= 0; layer--)
            {
                for (var neuron = 0; neuron < Neurons[layer].Length; neuron++)
                {
                    Neurons[layer][neuron].Error = 0;
                }

                // go through each neuron in next layer
                for (var nextLayerNeuron = 0; nextLayerNeuron < Neurons[layer + 1].Length; nextLayerNeuron++)
                {
                    // Adjust Bias
                    Neurons[layer + 1][nextLayerNeuron].Bias += Neurons[layer + 1][nextLayerNeuron].Error;

                    // Adjust Weights
                    for (var layerNeuron = 0; layerNeuron < Neurons[layer].Length; layerNeuron++)
                    {
                        var weight = Synapses[layer][layerNeuron][nextLayerNeuron].Weight;
                        Synapses[layer][layerNeuron][nextLayerNeuron].Weight += Neurons[layer + 1][nextLayerNeuron].Error * Neurons[layer][layerNeuron].Output * rate;
                        Neurons[layer][layerNeuron].Error +=
                            Neurons[layer][layerNeuron].DerivateProcess(Neurons[layer][layerNeuron].Output) *
                            Neurons[layer + 1][nextLayerNeuron].Error *
                            weight;
                    }
                }
            }
        }

        public double CalculateCost(double[] expected)
        {
            return CalculateError(LayerCount - 1, expected);
        }

        private double CalculateError(int layer, double[] expected)
        {
            var layerLength = Neurons[layer].Length;
            if (expected.Length != layerLength)
                throw new ArgumentException();

            var result = 0d;
            for (var i = 0; i < layerLength; i++)
                result += Math.Pow(Neurons[LayerCount - 1][i].Output - expected[i], 2);

            return result;
        }
    }
}
