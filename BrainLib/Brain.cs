using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainLib
{
    public class Brain : IActivationFunction
    {
        private int _layer;
        private int _inputNeurons;
        private int _outputNeurons;
        private readonly double[][] _neurons;
        private readonly double[][][] _weights;

        public Brain(int layer)
        {
            _layer = layer;
            _neurons = new double[_layer][];
            _weights = new double[_layer][][];
            ActivationFunction = this;
        }
        public IActivationFunction ActivationFunction { get; set; }
        public void SetNeurons(int layer, int neurons)
        {
            _neurons[layer] = new double[neurons];
            if (layer == 0)
                _inputNeurons = neurons;
            else if (layer == _layer - 1)
                _outputNeurons = neurons;
        }
        public void SetupWeights()
        {
            for (var layer = 0; layer < _layer - 1; layer++)
            {
                var length1 = _neurons[layer].Length;
                var length2 = _neurons[layer + 1].Length;
                _weights[layer] = new double[length1][];

                for (var neuron1 = 0; neuron1 < length1; neuron1++)
                {
                    _weights[layer][neuron1] = new double[length2];
                    for (var neuron2 = 0; neuron2 < length2; neuron2++)
                    {
                        _weights[layer][neuron1][neuron2] = 0;
                    }
                }
            }
        }
        public void SetupWeights(WeightSetDelegate weight)
        {
            for (var layer = 0; layer < _layer-1; layer++)
            {
                var length1 = _neurons[layer].Length;
                var length2 = _neurons[layer + 1].Length;
                _weights[layer] = new double[length1][];

                for (var neuron1 = 0; neuron1 < length1; neuron1++)
                {
                    _weights[layer][neuron1] = new double[length2];
                    for (var neuron2 = 0; neuron2 < length2; neuron2++)
                    {
                        _weights[layer][neuron1][neuron2] = weight(layer, neuron1, neuron2, _weights[layer][neuron1][neuron2]);
                    }
                }
            }
        }
        public double[] FeedForward(double[] values)
        {
            if(values.Length > _inputNeurons) 
                throw new ArgumentOutOfRangeException(nameof(values));

            Array.Copy(values, _neurons[0], _inputNeurons);
            for (var layer = 1; layer < _layer; layer++)
            {
                for (var neuron2 = 0; neuron2 < _neurons[layer].Length; neuron2++)
                {
                    var value = 0d;
                    for (var neuron1 = 0; neuron1 < _neurons[layer-1].Length; neuron1++)
                    {
                        value += _neurons[layer - 1][neuron1] * _weights[layer - 1][neuron1][neuron2];
                    }

                    _neurons[layer][neuron2] = ActivationFunction.Calculate(value);
                }
            }

            return _neurons[_neurons.Length - 1];
        }
        public double Calculate(double value)
        {
            return Math.Tanh(value);
        }
    }
}
