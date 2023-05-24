# GEAR: Gaze-enabled Augmented Reality for Human Activity Recognition

*Supplementary Material of the publication*:

Kenan Bektaş, Jannis Strecker, Simon Mayer, Kimberly Garcia, Jonas Hermann, Kay Erik Jenss, Yasmine Sheila Antille, and Marc Elias Solèr. 2023. GEAR: Gaze-enabled augmented reality for human activity recognition. In 2023 Symposium on Eye Tracking Research and Applications (ETRA ’23), May 30-June 2, 2023, Tübingen, Germany. ACM, New York, NY, USA, 9 pages. https://doi.org/10.1145/3588015.358840

## 📹 Demo Video 

The file [`GEAR_Video.mp4`](https://github.com/Interactions-HSG/GEAR/blob/main/GEAR-4-HAR/GEAR_Video.mp4) shows a video of all three activities being recognized by GEAR.


## 👀 GazeData 

The raw gaze data collected with the Microsoft HoloLens 2 and the Pupil Core can be found in their respective folders: [/GazeData/RawGazeDataFromHL2](https://github.com/Interactions-HSG/GEAR/tree/main/GEAR-4-HAR/GazeData/RawGazeDataFromHL2) and [/GazeData/RawGazeDataFromPupilCore](https://github.com/Interactions-HSG/GEAR/tree/main/GEAR-4-HAR/GazeData/RawGazeDataFromPupilCore).


## 🚀 AR Application 

The folder [/UnityApp](https://github.com/Interactions-HSG/GEAR/tree/main/GEAR-4-HAR/UnityApp) contains the AR application. See the README file in the folder for further instructions.


## 📈 Activity Recognition 

This folder contains 3 Jupyter Notebooks for the classification of activities from gaze data: 

- [*FeatureCalculation.ipynb*](https://github.com/Interactions-HSG/GEAR/blob/main/GEAR-4-HAR/FeatureCalculation.ipynb) - This notebook  contains the Event Detection and Feature Calculation.

- [*AnSVMClassifierForHL2GazeFeatures*](https://github.com/Interactions-HSG/GEAR/blob/main/GEAR-4-HAR/AnSVMClassifierForHL2GazeFeatures.ipynb) - This notebook contains the SVM-based classification model.
- [*RandomForestClassifier*](https://github.com/Interactions-HSG/GEAR/blob/main/GEAR-4-HAR/RandomForestClassifier.ipynb)- This notebook contains the RandomForest-based classification model.
- [*ExtraTreesClassifier*](https://github.com/Interactions-HSG/GEAR/blob/main/GEAR-4-HAR/ExtraTreesClassifier.ipynb) - This notebook contains the ExtremelyRandomizedTrees-based classification model.
- [*OnlineFeaturesCalculation.ipynb*](https://github.com/Interactions-HSG/GEAR/blob/main/GEAR-4-HAR/OnlineFeaturesCalculation.ipynb) - This notebook is an adaption of the offline feature calculation and includes references to the other two notebooks. It needs to be used with the Unity Application.

Note that the code for including the privacy-preserving personal datastores (Solid Pods) is not included in the supplementary material. An anonymized version of the code would hide important parts of the code and therefore make it difficult to understand. Upon publication of our paper, this code will  be included in a non-anonymized way.


## 📧 Contact  
Kenan Bektaş: [kenan.bektas@unisg.ch](mailto:kenan.bektas@unisg.ch)

This research has been done by the group of Interaction- and Communication-based Systems ([interactions.unisg.ch](https://interactions.unisg.ch)) at the University of St.Gallen ([unisg.ch](https://unisg.ch)).

## 📑 Reference 
```bibtex
@inproceedings{bektas2023,
  title = {{{GEAR}}: {{Gaze-enabled}} Augmented Reality for Human Activity Recognition},
  booktitle = {{{ACM Symposium}} on {{Eye Tracking Research}} and {{Applications}}},
  author = {Bektaş, Kenan and Strecker, Jannis and Mayer, Simon and Garcia, Kimberly and Hermann, Jonas and Jenss, Kay Erik and Antille, Yasmine Sheila and Solèr, Marc Elias},
  date = {2023},
  series = {{{ETRA}} '23},
  publisher = {{ACM}},
  location = {{New York, NY, USA}},
  doi = {10.1145/3588015.3588402},
  url={https://www.alexandria.unisg.ch/handle/20.500.14171/107618},
  abstract = {Head-mounted Augmented Reality (AR) displays overlay digital information on physical objects. Through eye tracking, they allow novel interaction methods and provide insights into user attention, intentions, and activities. However, only few studies have used gaze-enabled AR displays for human activity recognition (HAR). In an experimental study, we collected gaze data from 10 users on a HoloLens 2 (HL2) while they performed three activities (i.e., read, inspect, search). We trained machine learning models (SVM, Random Forest, Extremely Randomized Trees) with extracted features and achieved an up to 98.7\% activity-recognition accuracy. On the HL2, we provided users with an AR feedback that is relevant to their current activity. We present the components of our system (GEAR) including a novel solution to enable the controlled sharing of collected data. We provide the scripts and anonymized datasets which can be used as teaching material in graduate courses or for reproducing our findings.},
  eventtitle = {{{ETRA}} '23: 2023 {{Symposium}} on {{Eye Tracking Research}} and {{Applications}}},
}
```

