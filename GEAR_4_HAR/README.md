# GEAR: Gaze-enabled Augmented Reality for Human Activity Recognition

*Supplementary Material of the publication*

Kenan Bektaş, Jannis Strecker, Simon Mayer, Kimberly Garcia, Jonas Her-
mann, Kay Erik Jenss, Yasmine Sheila Antille, and Marc Elias Solèr. 2023.
GEAR: Gaze-enabled augmented reality for human activity recognition. In
2023 Symposium on Eye Tracking Research and Applications (ETRA ’23), May
30-June 2, 2023, Tubingen, Germany. ACM, New York, NY, USA, 9 pages.
https://doi.org/10.1145/3588015.358840

## Demo Video

The file `GEAR_Video.mp4`shows a video of all three activities being recognized by by GEAR.

---

## Gaze Data

The raw gaze data collected with the Microsoft HoloLens 2 and the Pupil Core can be found in their respective folders.

---

## AR Application

The folder UnityApp contains the AR application. See the README file in the folder for further instructions.

---

## Activity Recognition

This folder contains 3 Jupyter Notebooks for the classification of activities from gaze data: 

- *FeatureCalculation.ipynb* - This notebook  contains the Event Detection and Feature Calculation.

- *AnSVMClassifierForHL2GazeFeatures* - This notebook contains the SVM-based classification model.

- *OnlineFeaturesCalculation.ipynb* - This notebook is an adaption of the offline feature calculation and includes references to the other two notebooks. It needs to be used with the Unity Application.

Note that the code for including the privacy-preserving personal datastores (Solid Pods) is not included in the supplementary material. An anonymized version of the code would hide important parts of the code and therefore make it difficult to understand. Upon publication of our paper, this code will  be included in a non-anonymized way. 
