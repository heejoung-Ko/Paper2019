import tensorflow as tf
import numpy as np
import gym
import random
import dqn
from collections import deque

env = gym.make('CartPole-v0')
env._max_episode_steps = 10001

# Parameter
input_size = env.observation_space.shape[0] # 4
output_size = env.action_space.n 			# 2 (왼쪽 오른쪽 두 방향)
dis = 0.99

REPLAY_MEMORY = 50000
MAX_EPISODE = 5000
BATSH_SIZE = 64

def replay_train(mainDQN, targetDQN, train_batch) :
	x_stack = np.empty(0).reshape(0, input_size)
	y_stack = np.empty(0).reshape(0, output_size)

	for state, action, reward, next_state, done in train_batch :
		Q = mainDQN.predict(state)

		if done :
			Q[0, action] = reward

		else :
			Q[0, action] = reward + dis * np.max(targetDQN.predict(next_state))

		# vstak = vertical stack
		y_stack = np.vstack([y_stack, Q])		#  y_stack |  x_stack
		x_stack = np.vstack([x_stack, state])	#    Q     |   state

	return mainDQN.update(x_stack, y_stack)

def bot_play(mainDQN) :
	state = env.reset()
	reward_sum = 0

	while True:
		env.render()
		action = np.argmax(mainDQN.predict(state))
		state, reward, done, _ = env.step(action)
		reward_sum += reward

		if done :
			print("Total score : {}".format(reward_sum))
			break

def get_copy_var_ops(*, dest_scope_name = "target", src_scope_name = "main") :
	op_holder = []

	src_vars = tf.get_collection(tf.GraphKeys.TRAINABLE_VARIABLES, scope = src_scope_name)
	dest_vars = tf.get_collection(tf.GraphKeys.TRAINABLE_VARIABLES, scope = dest_scope_name)

	for src_var, dest_var in zip(src_vars, dest_vars) :
		op_holder.append(dest_var.assign(src_var.value()))

		return op_holder



def main() :
	replay_buffer = deque()

	with tf.Session() as sess :
		mainDQN = dqn.DQN(sess, input_size, output_size, name = "main")
		targetDQN = dqn.DQN(sess, input_size, output_size, name = "target")
		tf.global_variables_initializer().run()

		copy_ops = get_copy_var_ops(dest_scope_name = "target", 
									src_scope_name = "main")

		sess.run(copy_ops)

		for episode in range(MAX_EPISODE) :
			e = 1. / ((episode / 10) + 1)
			done = False
			step_count = 0

			state = env.reset()

			while not done :
				if np.random.rand(1) < e :
					action = env.action_space.sample()
				else :
					action = np.argmax(mainDQN.predict(state))

				# Get new state, reward
				next_state, reward, done, _ = env.step(action)

				if done :
					reward = -100

				replay_buffer.append((state, action, reward, next_state, done))
				
				# 버퍼에 무한정 쌓이지 않게 크기를 정하고 넘어가면 pop한다~
				if len(replay_buffer) > REPLAY_MEMORY :
					replay_buffer.popleft()

				state = next_state
				step_count += 1

				if step_count > 10000 :
					break

			print("Episode : {}  steps : {}".format(episode, step_count))

			if step_count > 10000 :
				pass

			if episode % 10 == 1 :
				for _ in range(50) :
					minibatch = random.sample(replay_buffer, 10)
					loss, _ = replay_train(mainDQN, targetDQN, minibatch)
				
				print("Loss : ", loss)
				sess.run(copy_ops)

		bot_play(mainDQN)


if __name__ == "__main__" :
	main()
