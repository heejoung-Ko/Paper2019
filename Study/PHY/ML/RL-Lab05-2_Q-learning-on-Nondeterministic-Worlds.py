import gym
from gym.envs.registration import register
from colorama import init
import random
import matplotlib.pyplot as plt
import numpy as np

def rargmax(vector) :
	m = np.amax(vector)
	indices = np.nonzero(vector == m)[0]
	return random.choice(indices)

init(autoreset = True)

env = gym.make('FrozenLake-v0')
env.reset()
env.render()

Q = np.zeros([env.observation_space.n, env.action_space.n]) # 16칸, 4가지 방향
learning_rate = .85
dis = .99 # discount reward parameter
num_episodes = 2000

rList = []
for i in range(num_episodes) :
	state = env.reset()
	rAll = 0
	done = False

	while not done :
		# add random noise
		action = np.argmax(Q[state, :] + np.random.randn(1, env.action_space.n) / (i + 1))

		new_state, reward, done, _ = env.step(action)

		# Update Q-Table with new knowledge using decay rate
		Q[state, action] = (1 - learning_rate) * Q[state, action] \
						   + learning_rate * (reward + dis * np.max(Q[new_state, :]))

		rAll += reward
		state = new_state

	rList.append(rAll)

print("Success rate : " + str(sum(rList)/num_episodes))
print("Final Q-Table Values")
print("LEFT DOWN RIGHT UP")
print(Q)
plt.bar(range(len(rList)), rList, color = "blue")
plt.show()