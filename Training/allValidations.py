# -*- coding: utf-8 -*-
"""
Created on Sun Feb 11 15:41:53 2024

@author: Stella
"""

import pandas as pd
import numpy as np
from sklearn.model_selection import train_test_split, cross_val_score, KFold
from sklearn.neighbors import RandomForestClassifier
from sklearn.impute import SimpleImputer
from sklearn.metrics import confusion_matrix, accuracy_score
from sklearn.preprocessing import StandardScaler
import matplotlib.pyplot as plt

# Load your CSV file into a pandas DataFrame
csv_file_path = 'data1000_2.csv'

# Use the error_bad_lines parameter to skip lines with inconsistent numbers of fields
data = pd.read_csv(csv_file_path, on_bad_lines='warn', skip_blank_lines=False, na_values='', delimiter=';', decimal=',')

# Print the column names to inspect them
print("First Column Name:", data.columns[0])

# Separate features (X) and target variable (y)
X = data.drop('class', axis=1)
y = data['class']

# Use SimpleImputer to handle missing values with a constant
constant_imputer = SimpleImputer(strategy='constant', fill_value=0)  # You can change fill_value to any constant value
X_imputed = pd.DataFrame(constant_imputer.fit_transform(X), columns=X.columns)

# Split the dataset into training and testing sets
X_train, X_test, y_train, y_test = train_test_split(X_imputed, y, test_size=0.2, random_state=42)

# Check the distribution of classes in y_train
print(y_train.value_counts())

# Standardize the features (optional but can improve performance)
scaler = StandardScaler()
X_train_scaled = scaler.fit_transform(X_train)
X_test_scaled = scaler.transform(X_test)

# Create a RandomForest classifier
model = RandomForestClassifier()

# Train the model
model.fit(X_train_scaled, y_train)
    
# Make predictions on the test set
y_pred = model.predict(X_test_scaled)

# Evaluate the model
accuracy = accuracy_score(y_test, y_pred)
conf_matrix = confusion_matrix(y_test, y_pred)

print(f'Accuracy: {accuracy}')

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
    accuracy = model.score(X_test_scaled, y_test)
    accuracies.append(accuracy)

average_accuracy_subsample = np.mean(accuracies)
print("Average accuracy with random subsampling:", average_accuracy_subsample)

# Leave-one-out
# from sklearn.model_selection import LeaveOneOut

# loo = LeaveOneOut()
# scores = cross_val_score(model, X_imputed, y, cv=loo)
# ("Accuracy with leave-one-out:", scores.mean())
