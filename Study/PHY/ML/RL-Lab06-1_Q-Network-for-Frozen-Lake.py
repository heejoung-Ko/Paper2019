import tensorflow as tf
import numpy as np
import gym
import matplotlib.pyplot as plt

def one_hot(x) :
	return np.identity(16)[x : x + 1]

env = gym.make('FrozenLake-v0')

input_size = env.observation_space.n
output_size = env.action_space.n
learning_rate = 0.1

X = tf.placeholder(shape=[1, input_size], dtype = tf.float32)
W = tf.Variable(tf.random_uniform([input_size, output_size], 0, 0.01)) # 16칸, 4가지 방향

Qpred = tf.matmul(X, W)
Y = tf.placeholder(shape = [1, output_size], dtype = tf.float32)

loss = tf.reduce_sum(tf.square(Y - Qpred))

train = tf.train.GradientDescentOptimizer(learning_rate = learning_rate).minimize(loss)
dis = .99 # discount reward parameter
num_episodes = 2000

rList = []
with tf.Session() as sess : 
	sess.run(tf.global_variables_initializer())
	for i in range(num_episodes) :
		state = env.reset()
		e = 1. / ((i / 50) + 10)
		rAll = 0
		done = False
		local_loss = []

		while not done :
			Qs = sess.run(Qpred, feed_dict = {X : one_hot(state)})

			if np.random.rand(1) < e :
				action = env.action_space.sample()
			else : 
				action = np.argmax(Qs)

			new_state, reward, done, _ = env.step(action)

			if done :
				# Update Q
				Qs[0, action] = reward
			else :
				Qs1 = sess.run(Qpred, feed_dict = {X : one_hot(new_state)})
				Qs[0, action] = reward + dis * np.max(Qs1)

			sess.run(train, feed_dict = {X : one_hot(state), Y : Qs})
		
			rAll += reward
			state = new_state

		rList.append(rAll)

print("Success rate : " + str(sum(rList)/num_episodes))

plt.bar(range(len(rList)), rList, color = "blue")
plt.show()