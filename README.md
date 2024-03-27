# ML_classification_VR_controller
ML models and shallow NN for binary classification of VR controller data

Data*
The data in raw form, and pre-processed for data structure and readability. The two scripts move the class column to the start of the document, and add headers to the csv columns, respectively.

Training*
In allValidations.py, the classifier RandomForest is used, and is checked with 3 validation methods.
In keras1.py, a shallow neural network is made using Keras TensorFlow libraries and is checked with 2 validation methods. Optionally the model is converted to .onnx format for the later use.
The LOO method is very expensive and is being commented out/ommited.
