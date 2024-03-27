# -*- coding: utf-8 -*-
"""
Created on Wed Jan 31 20:55:32 2024

@author: Stella
"""

import pandas as pd
import numpy as np
import matplotlib.pyplot as plt
import tensorflow as tf
from sklearn.impute import SimpleImputer
from sklearn.model_selection import train_test_split, cross_val_score, KFold
from sklearn.preprocessing import StandardScaler
from tensorflow.keras.models import Sequential
from tensorflow.keras.layers import Dense
import tf2onnx
import onnx

# Load the dataset
csv_file_path = 'data1000_2.csv'
df = pd.read_csv(csv_file_path, on_bad_lines='warn', skip_blank_lines=False, na_values='', delimiter=';', decimal=',')

# Display the first few rows to understand the data
print(df.head())

# Extract features and labels
X = df.drop('class', axis=1)
y = df['class']

# Use SimpleImputer to handle missing values with a constant
constant_imputer = SimpleImputer(strategy='constant', fill_value=0)
X_imputed = pd.DataFrame(constant_imputer.fit_transform(X), columns=X.columns)

# Split the data into training and testing sets
X_train, X_test, y_train, y_test = train_test_split(X_imputed, y, test_size=0.2, random_state=42)

# Further split the training data into training and validation sets
X_train, X_val, y_train, y_val = train_test_split(X_train, y_train, test_size=0.2, random_state=42)

# Standardize the features
scaler = StandardScaler()
X_train = scaler.fit_transform(X_train)
X_val = scaler.transform(X_val)
X_test = scaler.transform(X_test)

# Define the model
model = Sequential()
model.add(Dense(64, activation='relu', input_shape=(X_train.shape[1],)))
model.add(Dense(32, activation='relu'))
model.add(Dense(1, activation='sigmoid'))

# Compile the model
model.compile(optimizer='adam', loss='binary_crossentropy', metrics=['accuracy'])

# Train the model
history = model.fit(X_train, y_train, epochs=10, batch_size=32, validation_data=(X_val, y_val))

# Evaluate the model
loss, accuracy = model.evaluate(X_test, y_test)
print(f'Accuracy on the test set: {accuracy}')

# k-fold cross-validation
k_fold = KFold(n_splits=5, shuffle=True, random_state=42)
accuracy_k_fold = np.mean(cross_val_score(model, X_imputed, y, cv=k_fold))
print("Accuracy with k-fold cross-validation:", accuracy_k_fold)

# Random subsampling
num_iterations = 10
subsample_size = 0.8
accuracies = []

for _ in range(num_iterations):
    indices = np.random.choice(len(X), int(len(X) * subsample_size), replace=False)
    X_subsample = X_imputed.iloc[indices]
    y_subsample = y.iloc[indices]
    model.fit(X_subsample, y_subsample)
    accuracy = model.score(X_test, y_test)
    accuracies.append(accuracy)

average_accuracy_subsample = np.mean(accuracies)
print("Average accuracy with random subsampling:", average_accuracy_subsample)

# Make predictions
predictions = model.predict(X_test)

# Get training history
training_accuracy = history.history['accuracy']
validation_accuracy = history.history['val_accuracy']

# Create plot
plt.plot(training_accuracy, label='Training Accuracy')
plt.plot(validation_accuracy, label='Validation Accuracy')
plt.xlabel('Epoch')
plt.ylabel('Accuracy')
plt.title('Training and Validation Accuracy')
plt.legend()
plt.show()

# # Convert the Keras model to ONNX format
# onnx_model, _ = tf2onnx.convert.from_keras(model)

# # Save the ONNX model to a file
# onnx_filename = 'keras2.onnx'
# onnx.save(onnx_model, onnx_filename)
