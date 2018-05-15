Knapsack Decision Task used without imaging (Just behavioural). Unity 3D (C#) code.
This version (T4) of KS-Decision differs from T1 in that players can submit their answers before time is up. Additionally, the question for each instance is shown from the beginning of the trial.

Game controls:
- Start screen with SPACE
- UP arrow to skip to answer screen.
- Answer buttons are for left and right arrows for left and right answers respectively.

Input/Output data folders are located in Assets/DataInf. This folder has to be added manually to the game after building. 

This is the structure of the folder:
- DataInf
	- Output
	- Input 
		- layoutParam.txt
		- param.txt
		- KPInstances
			- i1.txt
			- i2.txt 
				…
			- 1_param2.txt
			- 2_param2.txt
				…

Description of INPUT files:




Input Files: param.txt, n_param2.txt, layoutParam.txt, KPInstances/i1.txt…

The main structure of these files is: 
NameOfTheVariable1:Value1
NameOfTheVariable2:Value2
…

layoutParam.txt
Relevant Parameters for the layout of the screen. All Variables must be INTEGERS.
columns:=number of columns in the grid were to lay randomly the items.
rows:= number of rows in the grid were to lay randomly the items.
randomPlacementType:=The method to be used to place items randomly on the grid.
	1:Choose random positions from full grid. It might happen that no placement is 		found and the trial will be skipped.
	2. Choose randomly out of 10 positions. A placement is guaranteed. For this 		placement type it must be that columns=16 and rows=12. Alternatively, delete these 	variables from the layout.txt file
totalAreaBill:=The total area of all the value items. A good initialisation for this 		variables is the number of items plus 1.
totalAreaWeight:=The total area of all the weight items. A good initialisation for this 	variables is the number of items plus 1.


param.txt
Relevant Parameters of the task. All Variables must be INTEGERS or vectors of INTEGERS.
timeRest1min:=Minimum time for the randomised inter-trial Break.
timeRest1max:=Maximum time for the randomised inter-trial Break.
timeRest2:=Time for the inter-blocks Break.
timeQuestion:=Time given for each trial.
timeAnswer:=Time given to answer.

KPInstances/n_param2.txt 
Variables can be allocated between param.txt and param2.txt with no effect on the game; however there must not be repeated definitions of variables. The distinction is done because param2.txt is an output from the instance selection program (e.g python).
numberOfInstances:=Number of instances to be imported. The files uploaded are 			automatically i1-i”numberOfInstances”
numberOfBlocks:=Number of blocks.
numberOfTrials:=Number of trials in each block.
instanceRandomization:=Sequence of instances to be randomised. The vector must have length: 	trials*blocks. E.g. [1,3,2,3,1,3,1,2,3,1] for 2 blocks of 5 trials.


KPInstances/i1.txt,KPInstances/i2.txt,…
Instance information. Each file is a different instance of the Knapsack problem. 
Files must be added sequentially (i.e. 1,2,3,…). All Variables must be INTEGERS or vectors of INTEGERS.
Weights and values must be vectors with the same length. InstanceType should be allocated according to one of the levels of difficulty. Sample Instance:

weights:[48,34,43,32,20,44]
values:[26,24,34,47,17,11]
capacity:90
profit:100
problemID:98-0.41-0.63
instanceType:1
solution:0

