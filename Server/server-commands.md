# Notes

* For each Trials task, there will be multipliers of [0.3, 1.7] for the training
  and [1.525, 1.75, 0.875, 1.125, 1.125, 1.375] for the main task.
* The interaction mode will automatically switch at the start of the second Nanotube task.

# Running a server

## The Python Server

To run a full game without recording:
```
nanover-omni --name "SubtleGame" --omm "..\Inputs\sandbox_2_C10_alkanes.xml" "..\Inputs\17-alanine.xml" "..\Inputs\nanotube-methane.xml" "..\Inputs\ANGLE\buckyball_angle_A_0.3.xml" "..\Inputs\ANGLE\buckyball_angle_A_0.625.xml" "..\Inputs\ANGLE\buckyball_angle_A_0.796.xml" "..\Inputs\ANGLE\buckyball_angle_A_0.89.xml" "..\Inputs\ANGLE\buckyball_angle_A_0.94.xml" "..\Inputs\ANGLE\buckyball_angle_A_1.06.xml" "..\Inputs\ANGLE\buckyball_angle_A_1.1105.xml" "..\Inputs\ANGLE\buckyball_angle_A_1.2036.xml" "..\Inputs\ANGLE\buckyball_angle_A_1.375.xml" "..\Inputs\ANGLE\buckyball_angle_A_1.7.xml" "..\Inputs\ANGLE\buckyball_angle_B_0.3.xml" "..\Inputs\ANGLE\buckyball_angle_B_0.625.xml" "..\Inputs\ANGLE\buckyball_angle_B_0.796.xml" "..\Inputs\ANGLE\buckyball_angle_B_0.89.xml" "..\Inputs\ANGLE\buckyball_angle_B_0.94.xml" "..\Inputs\ANGLE\buckyball_angle_B_1.06.xml" "..\Inputs\ANGLE\buckyball_angle_B_1.1105.xml" "..\Inputs\ANGLE\buckyball_angle_B_1.2036.xml" "..\Inputs\ANGLE\buckyball_angle_B_1.375.xml" "..\Inputs\ANGLE\buckyball_angle_B_1.7.xml"
```

If you want to record the session:
```
nanover-omni --name "SubtleGame" --omm "..\Inputs\sandbox_2_C10_alkanes.xml" "..\Inputs\17-alanine.xml" "..\Inputs\nanotube-methane.xml" "..\Inputs\ANGLE\buckyball_angle_A_0.3.xml" "..\Inputs\ANGLE\buckyball_angle_A_0.625.xml" "..\Inputs\ANGLE\buckyball_angle_A_0.796.xml" "..\Inputs\ANGLE\buckyball_angle_A_0.89.xml" "..\Inputs\ANGLE\buckyball_angle_A_0.94.xml" "..\Inputs\ANGLE\buckyball_angle_A_1.06.xml" "..\Inputs\ANGLE\buckyball_angle_A_1.1105.xml" "..\Inputs\ANGLE\buckyball_angle_A_1.2036.xml" "..\Inputs\ANGLE\buckyball_angle_A_1.375.xml" "..\Inputs\ANGLE\buckyball_angle_A_1.7.xml" "..\Inputs\ANGLE\buckyball_angle_B_0.3.xml" "..\Inputs\ANGLE\buckyball_angle_B_0.625.xml" "..\Inputs\ANGLE\buckyball_angle_B_0.796.xml" "..\Inputs\ANGLE\buckyball_angle_B_0.89.xml" "..\Inputs\ANGLE\buckyball_angle_B_0.94.xml" "..\Inputs\ANGLE\buckyball_angle_B_1.06.xml" "..\Inputs\ANGLE\buckyball_angle_B_1.1105.xml" "..\Inputs\ANGLE\buckyball_angle_B_1.2036.xml" "..\Inputs\ANGLE\buckyball_angle_B_1.375.xml" "..\Inputs\ANGLE\buckyball_angle_B_1.7.xml" --record "recording-file-name" --include-velocities --include-forces
```

## The Rust Server

To run a full game without recording:
```
.\nanover-cli.exe --name "SubtleGame" "..\Inputs\sandbox_2_C10_alkanes.xml" "..\Inputs\17-alanine.xml" "..\Inputs\nanotube-methane.xml" "..\Inputs\ANGLE\buckyball_angle_A_0.3.xml" "..\Inputs\ANGLE\buckyball_angle_A_0.625.xml" "..\Inputs\ANGLE\buckyball_angle_A_0.796.xml" "..\Inputs\ANGLE\buckyball_angle_A_0.89.xml" "..\Inputs\ANGLE\buckyball_angle_A_0.94.xml" "..\Inputs\ANGLE\buckyball_angle_A_1.06.xml" "..\Inputs\ANGLE\buckyball_angle_A_1.1105.xml" "..\Inputs\ANGLE\buckyball_angle_A_1.2036.xml" "..\Inputs\ANGLE\buckyball_angle_A_1.375.xml" "..\Inputs\ANGLE\buckyball_angle_A_1.7.xml" "..\Inputs\ANGLE\buckyball_angle_B_0.3.xml" "..\Inputs\ANGLE\buckyball_angle_B_0.625.xml" "..\Inputs\ANGLE\buckyball_angle_B_0.796.xml" "..\Inputs\ANGLE\buckyball_angle_B_0.89.xml" "..\Inputs\ANGLE\buckyball_angle_B_0.94.xml" "..\Inputs\ANGLE\buckyball_angle_B_1.06.xml" "..\Inputs\ANGLE\buckyball_angle_B_1.1105.xml" "..\Inputs\ANGLE\buckyball_angle_B_1.2036.xml" "..\Inputs\ANGLE\buckyball_angle_B_1.375.xml" "..\Inputs\ANGLE\buckyball_angle_B_1.7.xml"
```

If you want to record the session:
``````
.\nanover-cli.exe --name "SubtleGame" "..\Inputs\sandbox_2_C10_alkanes.xml" "..\Inputs\17-alanine.xml" "..\Inputs\nanotube-methane.xml" "..\Inputs\ANGLE\buckyball_angle_A_0.3.xml" "..\Inputs\ANGLE\buckyball_angle_A_0.625.xml" "..\Inputs\ANGLE\buckyball_angle_A_0.796.xml" "..\Inputs\ANGLE\buckyball_angle_A_0.89.xml" "..\Inputs\ANGLE\buckyball_angle_A_0.94.xml" "..\Inputs\ANGLE\buckyball_angle_A_1.06.xml" "..\Inputs\ANGLE\buckyball_angle_A_1.1105.xml" "..\Inputs\ANGLE\buckyball_angle_A_1.2036.xml" "..\Inputs\ANGLE\buckyball_angle_A_1.375.xml" "..\Inputs\ANGLE\buckyball_angle_A_1.7.xml" "..\Inputs\ANGLE\buckyball_angle_B_0.3.xml" "..\Inputs\ANGLE\buckyball_angle_B_0.625.xml" "..\Inputs\ANGLE\buckyball_angle_B_0.796.xml" "..\Inputs\ANGLE\buckyball_angle_B_0.89.xml" "..\Inputs\ANGLE\buckyball_angle_B_0.94.xml" "..\Inputs\ANGLE\buckyball_angle_B_1.06.xml" "..\Inputs\ANGLE\buckyball_angle_B_1.1105.xml" "..\Inputs\ANGLE\buckyball_angle_B_1.2036.xml" "..\Inputs\ANGLE\buckyball_angle_B_1.375.xml" "..\Inputs\ANGLE\buckyball_angle_B_1.7.xml" --state "recording-file-name.state" --trajectory "recording-file-name.traj" --include-velocity --include-forces
``````

