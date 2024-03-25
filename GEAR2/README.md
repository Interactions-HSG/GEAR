# GEAR2: Gaze-enabled Activity Recognition for Augmented Reality Feedback

This repository contains supplementary material of an EXTENDED version of the application and data described in the publication:

Kenan Bektaş, Jannis Strecker, Simon Mayer, Kimberly Garcia, Jonas Hermann, Kay Erik Jenss, Yasmine Sheila Antille, and Marc Elias Solèr. 2023. GEAR: Gaze-enabled augmented reality for human activity recognition. In 2023 Symposium on Eye Tracking Research and Applications (ETRA ’23), May 30-June 2, 2023, Tübingen, Germany. ACM, New York, NY, USA, 9 pages. https://doi.org/10.1145/3588015.3588402

See the folder [GEAR-4-HAR](https://github.com/Interactions-HSG/GEAR/tree/main/GEAR-4-HAR) for more information.

## 👀 Gaze Data

The raw gaze data collected with the Microsoft HoloLens 2 can be found here: [/Data/RawGazeData](https://github.com/Interactions-HSG/GEAR/tree/main/GEAR2/Data/RawGazeData).

## 🚀 AR Application

The folder [/GEAR2-Unity-App](https://github.com/Interactions-HSG/GEAR/tree/main/GEAR2/GEAR2-Unity-App) contains the AR application for the Microsoft HoloLens 2. See the README file in the folder for further instructions.

## 📈 Activity Recognition

This folder contains 3 Jupyter Notebooks for the classification of activities from gaze data: 

- [*FeatureCalculation.ipynb*](https://github.com/Interactions-HSG/GEAR/blob/main/GEAR-4-HAR/FeatureCalculation.ipynb) - This notebook  contains the Event Detection and Feature Calculation.

- [*AnSVMClassifierForHL2GazeFeatures*](https://github.com/Interactions-HSG/GEAR/blob/main/GEAR-4-HAR/AnSVMClassifierForHL2GazeFeatures.ipynb) - This notebook contains the SVM-based classification model.

- [*RandomForestClassifier*](https://github.com/Interactions-HSG/GEAR/blob/main/GEAR-4-HAR/RandomForestClassifier.ipynb)- This notebook contains the RandomForest-based classification model.

- [*ExtraTreesClassifier*](https://github.com/Interactions-HSG/GEAR/blob/main/GEAR-4-HAR/ExtraTreesClassifier.ipynb) - This notebook contains the ExtremelyRandomizedTrees-based classification model.

- [*OnlineFeaturesCalculation.ipynb*](https://github.com/Interactions-HSG/GEAR/blob/main/GEAR-4-HAR/OnlineFeaturesCalculation.ipynb) - This notebook is an adaption of the offline feature calculation and includes references to the other two notebooks. It needs to be used with the Unity Application.

Note that the code for including the privacy-preserving personal datastores (Solid Pods) is not included in the supplementary material.

## 📧 Contact

Kenan Bektaş: [kenan.bektas@unisg.ch](mailto:kenan.bektas@unisg.ch)

This research has been conducted by the group of Interaction- and Communication-based Systems ([interactions.unisg.ch](https://interactions.unisg.ch)) at the University of St.Gallen ([unisg.ch](https://unisg.ch)).
