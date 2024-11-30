# How to train a model

## Step 1: Build the game

### IMPORTANT:

**(SAVE THE RESULT IN `phase2/envs`)**

https://unity-technologies.github.io/ml-agents/Learning-Environment-Executable/

## Step 2: Train the model

1. Go to the phase2 folder (where this file is located)
2. You should have save the executable you built in step 1 in the `envs` folder (`phase2/envs`)
3. Activate the conda environment with `conda activate mlagents`
4. Run the following command:

```bash
mlagents-learn <trainer-config-file> --env=./envs/<env_name> --run-id=<run-identifier> --num-envs=<num-of-game-instances> --no-graphics
```

Example:

```bash
mlagents-learn ./SoccerTwos.yaml --env=./envs/Base --run-id=base_01  --num-envs=5 --no-graphics
```

### Notes:

- The value for `--num-envs` should be the number of game instances you want to run in parallel. The value that worked best for me was 5, but this is gonna depend on your machine. There's no limitation, so if you select `100`, for example, it will probably crash your computer.
- In case you want to replace a previously trained model, you can use the same `run-id` and the flag `--force`.
- In case you want to continue training a previously trained model, you can use the same `run-id` and the flag `--resume`.
- To create a new model from an existing one, you can use the `--initialize-from <run-identifier>` flag.

## Step 3: Evaluate the model

Run the following command:

```bash
tensorboard --logdir results
```

Then open your browser and go to `http://localhost:6006/`. You should see the training results.

## Step 4: Test the model

1. Go to the phase2 folder (where this file is located)
2. Go to `/results/<run-identifier>` and copy the `<behavior-name>.onnx` file to the `Project/Assets/ML-Agents/Examples/SoccerNew/TFModels/` folder. (This path is relative to the repository root.)
3. Also change the `.onnx` file name in advance, if you don't want to overwrite the existing model.
