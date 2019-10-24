using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetworkLib;
using NetworkLib.Model.Neurons;
using NetworkLib.Model.Synapses;

namespace NetworkTest
{
    public class SimpleNetwork
    {
        private readonly INeuronalNetwork _network;
        private readonly List<INeuron> _inputNeurons = new List<INeuron>();
        private readonly List<INeuron> _outputNeurons = new List<INeuron>();
        private readonly List<ISynapse> _synapses = new List<ISynapse>();

        private readonly Func<double, double> _layerFunc;
        private readonly Func<double, double> _outputFunc;

        private double[] _inputValues;

        public SimpleNetwork(INeuronalNetwork network, Func<double, double> layerFunc, Func<double, double> outputFunc)
        {
            _network = network;
            _layerFunc = layerFunc;
            _outputFunc = outputFunc;
        }

        public void Init(int[] layers)
        {
            var neurons = new INeuron[layers.Length][];
            _inputValues = new double[layers[0]];

            for (var i = 0; i < layers.Length; i++)
            {
                var layerSize = layers[i];
                neurons[i] = new INeuron[layerSize];

                for (var j = 0; j < layerSize; j++)
                {
                    INeuron neuron;
                    if (i == 0)
                    {
                        neuron = new InputNeuron();
                        _inputNeurons.Add(neuron);
                    }
                    else if(i < layers.Length - 1)
                    {
                        neuron = new FunctionNeuronProcess(_layerFunc);
                    }
                    else
                    {
                        neuron = new OutputNeuron(_outputFunc);
                        _outputNeurons.Add(neuron);
                    }

                    neurons[i][j] = neuron;

                    _network.AddNeuron(neuron);

                    if (i > 0)
                    {
                        for (var k = 0; k < layers[i- 1]; k++)
                        {
                            var synapse = new Synapse(neurons[i - 1][k], neuron);
                            synapse.Weight = 1;

                            _synapses.Add(synapse);
                            _network.AddSynapse(synapse);
                        }
                    }
                }
            }
        }

        public void SetInputValue(int index, double value)
        {
            _inputValues[index] = value;
        }

        public void Process()
        {
            _network.Reset();

            for (int i = 0; i < _inputNeurons.Count; i++)
            {
                _inputNeurons[i].Value = _inputValues[i];
                _inputValues[i] = 0;
            }

            _network.FeedForward();
        }

        public double GetOutputValue(int index)
        {
            return _outputNeurons[index].Value;
        }
    }
}
