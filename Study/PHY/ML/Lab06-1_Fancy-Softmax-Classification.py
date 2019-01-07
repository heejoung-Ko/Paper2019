import tensorflow as tf
import numpy as np

xy = np.loadtxt('data-04-zoo.csv', delimiter = ',', dtype = np.float32)
x_data = xy[:, 0:-1]
y_data = xy[:, [-1]]

nb_classes = 7

X = tf.placeholder(tf.float32, [None, 16])
# one_hot을 적용하면 [[0], [3]] => [[1000000], [0001000]] 이렇게 됨
Y = tf.placeholder(tf.int32, [None, 1]) # 0 ~ 6까지의 값을 가짐
Y_one_hot = tf.one_hot(Y, nb_classes) # one_hot을 하면 차원이 한차원 더해지기 때문에 오류가 난다!
Y_one_hot = tf.reshape(Y_one_hot, [-1, nb_classes]) # 그래서 reshape를 통해서 다시 차원을 원래대로 돌려줘야한다!

W = tf.Variable(tf.random_normal([16, nb_classes]), name = 'weight')
b = tf.Variable(tf.random_normal([nb_classes]), name = 'bias')

logits = tf.matmul(X, W) + b
hypothesis = tf.nn.softmax(logits)

cost_i = tf.nn.softmax_cross_entropy_with_logits(logits = logits, labels = Y_one_hot)
cost = tf.reduce_mean(cost_i)

optimizer = tf.train.GradientDescentOptimizer(learning_rate = 0.1).minimize(cost)

prediction = tf.argmax(hypothesis, 1)
correct_prediction = tf.equal(prediction, tf.argmax(Y_one_hot, 1))
accuracy = tf.reduce_mean(tf.cast(correct_prediction, tf.float32))

with tf.Session() as sess :
	sess.run(tf.global_variables_initializer())

	for step in range(2000) : 
		sess.run(optimizer, feed_dict = {X : x_data, Y : y_data})

		if step % 100 == 0 :
			loss, acc = sess.run([cost, accuracy], feed_dict = {X : x_data, Y : y_data})

			print("Step : {:5}\tLoss : {:.3f}\tAcc : {:2%}".format(step, loss, acc))


	pred = sess.run(prediction, feed_dict = {X : x_data})

	for p, y in zip(pred, y_data.flatten()) :
		print("[{}] Prediction : {} True Y : {}".format(p == int(y), p, int(y)))