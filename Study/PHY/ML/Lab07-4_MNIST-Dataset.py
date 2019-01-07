import tensorflow as tf
import numpy as np
import matplotlib.pyplot as plt
import random
from tensorflow.examples.tutorials.mnist import input_data

mnist = input_data.read_data_sets("MNIST_data/", one_hot = True) 
# one_hot 을 true로 주면 Y값을 읽어올 때 자동으로 one_hot으로 읽어온다

nb_classes = 10

# MNIST 데이터 이미지가 28 * 28 pixel이기 때문에 28 * 28 = 784
X = tf.placeholder(tf.float32, [None, 784])
# 0 ~ 9까지 있기 때문에 class는 10
Y = tf.placeholder(tf.float32, [None, nb_classes])
W = tf.Variable(tf.random_normal([784, nb_classes]), name = 'weight')
b = tf.Variable(tf.random_normal([nb_classes]), name = 'bias')

hypothesis = tf.nn.softmax(tf.matmul(X, W) + b) 
cost = tf.reduce_mean(-tf.reduce_sum(Y * tf.log(hypothesis), axis = 1))
optimizer = tf.train.GradientDescentOptimizer(learning_rate = 0.1).minimize(cost)

is_correct = tf.equal(tf.arg_max(hypothesis, 1), tf.arg_max(Y, 1))
accuracy = tf.reduce_mean(tf.cast(is_correct, tf.float32))


# 전체 데이터를 한번 다 학습시키는 것을 one epoch이라고 한다.
# 전체 데이터를 한번에 학습시키기에 양이 너무 많기 때문에 batch size로 나눠서 학습시킨다.
# 이번 예제는 100개씩 15번 학습시키면 전체 데이터를 학습시킬 수 있다~
training_epochs = 15
batch_size = 100


with tf.Session() as sess : 
	sess.run(tf.global_variables_initializer())

	for epoch in range(training_epochs):
		avg_cost = 0
		total_batch = int(mnist.train.num_examples / batch_size)

		for i in range(total_batch) :
			batch_xs, batch_ys = mnist.train.next_batch(batch_size)
			c, _ = sess.run([cost, optimizer], feed_dict = {X : batch_xs, Y : batch_ys})
			avg_cost += c / total_batch

		print('Epoch : ', '%04d' % (epoch + 1), 'cost = ', '{:.9f}'.format(avg_cost))


	# accuracy.eval() = sess.run()
	print("Accuracy : ", accuracy.eval(session = sess, feed_dict = {X : mnist.test.images, Y : mnist.test.labels}))

	r = random.randint(0, mnist.test.num_examples - 1)
	print("Label : ", sess.run(tf.arg_max(mnist.test.labels[r:r+1], 1)))
	print("Prediction : ", sess.run(tf.arg_max(hypothesis, 1),
		feed_dict = {X : mnist.test.images[r:r + 1]}))

	plt.imshow(mnist.test.images[r: r+1].reshape(28, 28), cmap = 'Greys', interpolation = 'nearest')
	plt.show()
