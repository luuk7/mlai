#!/bin/bash

mlagents-learn ./SoccerTwos.yaml --env=./envs/HeadTurn.app --run-id="$1" --num-envs=5 --no-graphics
