using UnityEngine;
//using Unity.MLAgents;
//using Unity.MLAgents.Sensors;
//using Unity.MLAgents.Actuators;
using TensorFlow;
using System;
using System.IO;
using System.Collections.Generic;
using Unity.Sentis;
using static MLprediction;
using JetBrains.Annotations;
using TMPro;
using MathNet.Numerics.LinearAlgebra;
using System.Linq;
using MathNet.Numerics.Statistics;
using MathNet.Numerics.LinearAlgebra.Double;

public class MLprediction : MonoBehaviour
{
    public ModelAsset modelAsset;
    private Model runtimeModel;
    IWorker worker;

    public GameObject mlResult;
    public Material red, green;

    public int content;
    public TextMeshProUGUI pred, perce;

    public Shooter shooterScript;
    public float perc, count, hits = 0f;
    public float threshold = 0.6f;

    public Animator animator;
    public bool predicted1 = true;
    public bool wrongPred = false;

    // Start is called before the first frame update
    void Start()
    {
        runtimeModel = ModelLoader.Load(modelAsset);
        worker = WorkerFactory.CreateWorker(BackendType.GPUCompute, runtimeModel);
    }

    public void PredictShotOutcome(float[] inputData)
    {
        // Convert the float array to a MathNet.Numerics vector of doubles
        var vector = DenseVector.OfArray(inputData.Select(x => (double)x).ToArray());

        // Perform standard scaling using MathNet.Numerics
        var mean = Statistics.Mean(vector);
        var stdDev = Statistics.StandardDeviation(vector);

        var standardizedVector = (vector - mean) / stdDev;

        // Convert the standardized vector back to float[]
        float[] standardizedArray = standardizedVector.Select(x => (float)x).ToArray();

        // Create a 3D tensor shape with size of inputData 
        TensorShape shape = new TensorShape(1, standardizedArray.Length);

        // Create a new tensor from the array
        TensorFloat tensor = new TensorFloat(shape, standardizedArray);

        worker.Execute(tensor);

        foreach (var outputName in runtimeModel.outputs)
        {
            count++;
            TensorFloat outputTensor = worker.PeekOutput(outputName) as TensorFloat;
            outputTensor.MakeReadable();
            float[] outputData = outputTensor.ToReadOnlyArray();

            float predf = outputData[0];
            Debug.Log(predf);
            int prediction = (predf >= threshold) ? 1 : 0;
            if (prediction == 1)
            {
                //mlResult.GetComponent<Renderer>().material = green;
                if (shooterScript.madeLastShot)
                    hits++;
                else wrongPred = true;
                //if (!predicted1)
                //{
                animator.SetTrigger("Mcorrect");
                //}
                predicted1 = true;
            }
            else if (prediction == 0)
            {
                if (!shooterScript.madeLastShot)
                    hits++;
                else wrongPred = true;
                //if (predicted1)
                //{
                animator.SetTrigger("Mwrong");
                //}
                predicted1 = false;
                //mlResult.GetComponent<Renderer>().material = red;
            }

            perc = Mathf.Round((hits / count) * 100f);
            perce.text = hits.ToString() + "/" + count.ToString();
            pred.text = perc.ToString();
        }

        tensor.Dispose();
    }

    static void StandardScale(float[] data)
    {
        // Calculate mean
        float mean = 0f;
        foreach (float value in data)
        {
            mean += value;
        }
        mean /= data.Length;

        // Calculate standard deviation
        float stdDev = 0f;
        foreach (float value in data)
        {
            stdDev += (value - mean) * (value - mean);
        }
        stdDev = (float)Math.Sqrt(stdDev / data.Length);

        // Standardize the data
        for (int i = 0; i < data.Length; i++)
        {
            data[i] = (data[i] - mean) / stdDev;
        }
    }

    public class FloatStack
    {
        public int content;
        string prin;

        private List<float> stack;

        public FloatStack()
        {
            stack = new List<float>();
        }

        public void Push(float value)
        {
            stack.Add(value);
        }

        public float[] GetStackAsArray(int cont)
        {
            List<float> floatList = new List<float>();
            float[] floatArray = new float[cont];

            if (stack.Count < cont)
            {
                for (int i = 0; i < cont; i++)
                {
                    if (i < stack.Count)
                    {
                        floatArray[i] = stack[i];
                    }
                    else
                    {
                        floatArray[i] = 0f;
                    }
                }
            }

            else if (stack.Count > cont)
            {
                for (int i = 0; i < cont; i++)
                {
                    floatArray[i] = stack[i];
                }
            }

            else if(stack.Count == cont)
            {
                floatArray = stack.ToArray();
            }

            for (int i = 0; i < 10; i++)
            {
                prin = prin + " " + floatArray[i].ToString();
            }

            return floatArray;
        }

        public void PrintFloatStack()
        {
            foreach (float value in stack)
            {
                Debug.Log(value + " ");
            }
        }

        public void ClearStack()
        {
            stack.Clear();
        }
    }
}
