# GEAR: Gaze-enabled Augmented Reality for Human Activity Recognition

*Supplementary Material of the publication*:

Kenan Bektaş, Jannis Strecker, Simon Mayer, Kimberly Garcia, Jonas Hermann, Kay Erik Jenss, Yasmine Sheila Antille, and Marc Elias Solèr. 2023. GEAR: Gaze-enabled augmented reality for human activity recognition. In 2023 Symposium on Eye Tracking Research and Applications (ETRA ’23), May 30-June 2, 2023, Tübingen, Germany. ACM, New York, NY, USA, 9 pages. https://doi.org/10.1145/3588015.358840

## Demo Video

The file [`GEAR_Video.mp4`](https://github.com/Interactions-HSG/GEAR/blob/main/GEAR-4-HAR/GEAR_Video.mp4) shows a video of all three activities being recognized by GEAR.


## GazeData

The raw gaze data collected with the Microsoft HoloLens 2 and the Pupil Core can be found in their respective folders: [/RawGazeDataFromHL2](https://github.com/Interactions-HSG/GEAR/tree/main/GEAR-4-HAR/GazeData/RawGazeDataFromHL2) and [/RawGazeDataFromPupilCore](https://github.com/Interactions-HSG/GEAR/tree/main/GEAR-4-HAR/GazeData/RawGazeDataFromPupilCore).


## AR Application

The folder [/UnityApp](https://github.com/Interactions-HSG/GEAR/tree/main/GEAR-4-HAR/UnityApp) contains the AR application. See the README file in the folder for further instructions.


## Activity Recognition

This folder contains 3 Jupyter Notebooks for the classification of activities from gaze data: 

- [*FeatureCalculation.ipynb*](https://github.com/Interactions-HSG/GEAR/blob/main/GEAR-4-HAR/FeatureCalculation.ipynb) - This notebook  contains the Event Detection and Feature Calculation.

- [*AnSVMClassifierForHL2GazeFeatures*](https://github.com/Interactions-HSG/GEAR/blob/main/GEAR-4-HAR/AnSVMClassifierForHL2GazeFeatures.ipynb) - This notebook contains the SVM-based classification model.

- [*OnlineFeaturesCalculation.ipynb*](https://github.com/Interactions-HSG/GEAR/blob/main/GEAR-4-HAR/OnlineFeaturesCalculation.ipynb) - This notebook is an adaption of the offline feature calculation and includes references to the other two notebooks. It needs to be used with the Unity Application.

Note that the code for including the privacy-preserving personal datastores (Solid Pods) is not included in the supplementary material. An anonymized version of the code would hide important parts of the code and therefore make it difficult to understand. Upon publication of our paper, this code will  be included in a non-anonymized way.


## Reference
```bibtex
@article{bektasetal2023,
    title={GEAR: Gaze-enabled augmented reality for human activity recognition.},
    author={Kenan Bektaş, Jannis Strecker, Simon Mayer, Kimberly Garcia, Jonas Hermann, Kay Erik Jenss, Yasmine Sheila Antille, and Marc Elias Solèr},
    journal={},
    volume={},
    number={},
    pages={},
    ISSN={},
    url={},
    DOI={https://doi.org/10.1145/3588015.358840},
    publisher={},
    month={},
    year={2023}
}
```
