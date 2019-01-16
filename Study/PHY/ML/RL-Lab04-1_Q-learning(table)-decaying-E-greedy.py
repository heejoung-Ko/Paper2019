import gym
from gym.envs.registration import register
from colorama import init
import readchar
import random
import matplotlib.pyplot as plt
import numpy as np

def rargmax(vector) :
	m = np.amax(vector)
	indices = np.nonzero(vector == m)[0]
	return random.choice(indices)

LEFT = 0
DOWN = 1
RIGHT = 2
UP = 3

arrow_keys = {
    '\x1b[A' : UP,
    '\x1b[B' : DOWN,
    '\x1b[C' : RIGHT,
    '\x1b[D' : LEFT
}

init(autoreset = True)

register(
	id = 'FrozenLake-v3', entry_point = 'gym.envs.toy_text:FrozenLakeEnv',
	kwargs={'map_name' : '4x4', 'is_slippery' : False}
)

env = gym.make('FrozenLake-v3')
env.render()

Q = np.zeros([env.observation_space.n, env.action_space.n]) # 16칸, 4가지 방향
num_episodes = 2000

rList = []
for i in range(num_episodes) :
	state = env.reset()
	rAll = 0
	done = False

	while not done :
		action = rargmax(Q[state, :]) # random argmax

		new_state, reward, done, _ = env.step(action)

		Q[state, action] = reward + np.max(Q[new_state, :])

		rAll += reward
		state = new_state

	rList.append(rAll)

print("Success rate : " + str(sum(rList)/num_episodes))
print("Final Q-Table Values")
print("LEFT DOWN RIGHT UP")
print(Q)
plt.bar(range(len(rList)), rList, color = "blue")
plt.show()