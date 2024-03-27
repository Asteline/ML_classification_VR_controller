using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using System.IO;
using System;
using static MLprediction;

public class Ball : MonoBehaviour
{
    public GameObject shooter;
    public Transform thumbs;
    public Shooter shooterScript;
    public MLprediction mlpred;

    public Material red, green;

    private XRController xrController;

    public InputDevice _rightController;

    private StreamWriter csvWriter;
    private StreamReader csvReader;

    [SerializeField]
    private GameObject xrControllerObject;

    private string line;
    public bool racketInvolved = true;

    //public float[,] reshapedInputData = new float[1, 330];
    private FloatStack floatStack;

    // Start is called before the first frame update
    void Start()
    {
        shooter = GameObject.Find("Shooter");
        xrController = GameObject.Find("Right Controller").GetComponent<XRController>();
        shooterScript = GameObject.Find("GameManager").GetComponent<Shooter>();
        mlpred = GameObject.Find("GameManager").GetComponent<MLprediction>();
        thumbs = shooterScript.thumbsU;

        if (!shooterScript.onlyTesting)
        {
            csvWriter = new StreamWriter("data.csv", true);
            csvWriter.Write(Environment.NewLine);
            csvWriter.Write(shooterScript.cameraX);
        }
        floatStack = new FloatStack();
        PushToStack(shooterScript.cameraX);
    }
    private void OnCollisionEnter(Collision collision)
    {
        // Check if the ball collides with a wall
        if (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Ground"))
        {
            if (racketInvolved)
            {
                //thumbs.GetComponent<Renderer>().material = red;
                if (!shooterScript.onlyTesting)
                {
                    csvWriter.Write(";0");
                }

                shooterScript.RemovePoints();
                Debug.Log("Ball collided with a wall!");

                OnShotEnded();
            }
            Destroy(gameObject);
        }

        // Check if the ball collides with the racket
        if (collision.gameObject.CompareTag("Racket"))
        {
            //thumbs.GetComponent<Renderer>().material = green;
            if (!shooterScript.onlyTesting)
            {
                csvWriter.Write(";1");
            }

            shooterScript.AddPoints();
            Debug.Log("Ball collided with the racket!");


            OnShotEnded();

        }
    }

    void PushToStack(float value)
    {
        floatStack.Push(value);
    }

    public void OnShotEnded()
    {
        racketInvolved = false;
        if (!shooterScript.onlyTesting)
        {
            csvWriter.Close();
        }
        //floatStack.PrintFloatStack();
        float[] stackArray = floatStack.GetStackAsArray(mlpred.content);
        if(!shooterScript.onlyTraining)
        {
            mlpred.PredictShotOutcome(stackArray);
        }
    }

    private void InitializeInputDevices()
    {
        InitializeInputDevice(InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Right, ref _rightController);
    }

    private void InitializeInputDevice(InputDeviceCharacteristics inputCharacteristics, ref InputDevice inputDevice)
    {
        List<InputDevice> inputDevices = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(inputCharacteristics, inputDevices);

        if (inputDevices.Count > 0)
        {
            inputDevice = inputDevices[0];
        }
    }
    private void LateUpdate()
    {
        if (!_rightController.isValid)
        {
            InitializeInputDevices();
        }

        if (racketInvolved)
        {
            if (_rightController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.deviceVelocity, out Vector3 rightVelocity))
            {
                // Get the position and rotation of the controller
                Vector3 controllerPosition = xrController.transform.position;
                Quaternion controllerRotation = xrController.transform.rotation;

                if(!shooterScript.onlyTesting)
                {
                    csvWriter.Write(";" + rightVelocity.magnitude + ";");
                    csvWriter.Write(controllerPosition.x + ";" + controllerPosition.y + ";" + controllerPosition.z + ";");
                    csvWriter.Write(controllerRotation.eulerAngles.x + ";" + controllerRotation.eulerAngles.y + ";" + controllerRotation.eulerAngles.z);
                }

                PushToStack(rightVelocity.magnitude);
                PushToStack(controllerPosition.x);
                PushToStack(controllerPosition.y);
                PushToStack(controllerPosition.z);
                PushToStack(controllerRotation.eulerAngles.x);
                PushToStack(controllerRotation.eulerAngles.y);
                PushToStack(controllerRotation.eulerAngles.z);

            }
        }
    }
}
