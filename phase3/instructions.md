# Instructions

## Step 1: Clone the repo and install mlagents (you probably have done this already)
Run
```
git clone https://github.com/luuk7/mlai.git
cd mlai
git switch phase-3
```
Then follow this guide: https://github.com/Unity-Technologies/ml-agents/blob/develop/docs/Installation.md

TLDR of the guide:
```
conda create -n mlagents python=3.10.12 && conda activate mlagents
pip3 install torch -f https://download.pytorch.org/whl/torch_stable.html
pip3 install -e ./ml-agents-envs
pip3 install -e ./ml-agents
```

## Step 2: Build the game

### IMPORTANT:

**SAVE THE RESULT IN `phase3/envs`**

**ON WINDOWS WHEN YOU BUILD IT WILL ASK YOU TO SELECT A FOLDER TO SAVE IT. YOU SHOULD CREATE IT IN `phase3/envs`. FOR EXAMPLE `phase3/envs/HeadTurn`**

https://unity-technologies.github.io/ml-agents/Learning-Environment-Executable/

## Step 3: Make a conda environment for running the resource monitoring script.

Open a command prompt where you have access to the `conda` command and administrator privileges. (On windows type "Anaconda Powershell Prompt" into the search bar and right-click -> Run as Administrator)

Now run
```
conda create --name monitor
conda activate monitor
conda install psutil
conda install -c conda-forge pynvml
```

## Step 4: Run the training while collecting the resource usage data.
You need to open two administrator conda prompts in this (`mlai/phase3`) folder. One should have the `mlagents` environment active and the other the `monitor` environment.

Prompt 1 example:
```
(mlagents) PS C:\dev\mlai\phase3>
```

Prompt 2 example:
```
(monitor) PS C:\dev\mlai\phase3>
```

Now in Prompt 1 run:
```
mlagents-learn <path to config> --env=<path to built environment> --run-id=<id of the run> --num-envs=<number of environments> --no-graphics
```
**IMPORTANT: The \<id of the run\> should clearly indicate which config you are using for the run, I strongly recommend you just use the name of the config file**

**IMPORTANT: Unless you are the person running the number of environments experiments, --num-envs should be =4**

Example Prompt 1 command:
```
mlagents-learn ./configs/batch_buffer/bs_bs_4096_40960.yaml --env=./envs/HeadTurn --run-id=bs_bs_4096_40960 --num-envs=4 --no-graphics
```

Finally, in Prompt 2 run:
```
python ./monitor_resources.py --run-id=<same run id as for mlagents>
```

**IMPORTANT: The --run-id must be the same as the one for mlagents**

Example Prompt 2 command:
```
python ./monitor_resources.py --run-id=bs_bs_4096_40960
```

## Step 5: Repeat Step 4 two more times.

You should do Step 4 three times in total, twice for the configs parameter specific configs and once for `./configs/base/SoccerTwos.yaml`
