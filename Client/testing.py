sim_file_name = ("RECORDINGS\\recordingWRONGMULTIPLIERFORTESTING-buckyball_angle_A_1.5.traj:..\\Inputs\\RECORDINGS"
             "\\recordingWRONGMULTIPLIERFORTESTING-buckyball_angle_A_1.5.state")

print(float(sim_file_name.split(".traj")[0].split("recording-buckyball_angle_")[1].split("_")[1]))