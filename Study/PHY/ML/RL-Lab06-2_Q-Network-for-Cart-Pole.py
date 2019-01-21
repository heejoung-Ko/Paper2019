import tensorflow as tf
import numpy as np
import gym
import matplotlib.pyplot as plt

env = gym.make('CartPole-v0')
env.reset()

# Parameter
num_episodes = 2000
input_size = env.observation_space.shape[0] # 4
output_size = env.action_space.n 			# 2 (왼쪽 오른쪽 두 방향)
learning_rate = 1e-1
dis = 0.9

X = tf.placeholder(tf.float32, [None, input_size], name = "input_x")
# First layer of weights
W1 = tf.get_variable("W1", shape = [input_size, output_size], 
	initializer = tf.contrib.layers.xavier_initializer())
Qpred = tf.matmul(X, W1)

Y = tf.placeholder(shape = [None, output_size], dtype = tf.float32)

loss = tf.reduce_sum(tf.square(Y - Qpred))
train = tf.train.AdamOptimizer(learning_rate = learning_rate).minimize(loss)

sess = tf.Session()
sess.run(tf.global_variables_initializer())
rList = []

for i in range(num_episodes) :
	e = 1. / ((i / 10) + 1)
	rAll = 0
	step_count = 0
	state = env.reset()
	done = False

	while not done : 
		step_count += 1
		x = np.reshape(state, [1, input_size])

		Qs = sess.run(Qpred, feed_dict = {X : x})

		if np.random.rand(1) < e :
			action = env.action_space.sample()
		else :
			action = np.argmax(Qs)

		new_state, reward, done, _ = env.step(action)

		if done :
			Qs[0, action] = -100
		else :
			x1 = np.reshape(new_state, [1, input_size])
			Qs1 = sess.run(Qpred, feed_dict = {X : x1})
			Qs[0, action] = reward + dis * np.max(Qs1)

		sess.run(train, feed_dict = {X : x, Y : Qs})
		state = new_state

	rList.append(step_count)
	print("Episode : {}  steps : {}".format(i, step_count))

	if len(rList) > 10 and np.mean(rList[-10:]) > 500 :
		break

observation = env.reset()
reward_sum = 0

while True :
	env.render()

	x = np.reshape(observation, [1, input_size])
	Qs = sess.run(Qpred, feed_dict = {X : x})
	action = np.argmax(Qs)

	observation, reward, done, _ = env.step(action)
	reward_sum += reward

	if done :
		print("Total score : {}".format(reward_sum))
		break