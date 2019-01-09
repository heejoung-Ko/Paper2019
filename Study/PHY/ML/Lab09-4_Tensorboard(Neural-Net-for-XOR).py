import tensorflow as tf
import numpy as np

x_data = np.array([[0, 0], [0, 1], [1, 0], [1, 1]], dtype = np.float32)
y_data = np.array([[0], [1], [1], [0]], dtype = np.float32)

X = tf.placeholder(tf.float32)
Y = tf.placeholder(tf.float32)

W1 = tf.Variable(tf.random_normal([2, 2]), name = 'weight1')
b1 = tf.Variable(tf.random_normal([2]), name = 'bias1')
layer1 = tf.sigmoid(tf.matmul(X, W1) + b1)

W2 = tf.Variable(tf.random_normal([2, 1]), name = 'weight2')
b2 = tf.Variable(tf.random_normal([1]), name = 'bias2')
hypothesis = tf.sigmoid(tf.matmul(layer1, W2) + b2)

cost = -tf.reduce_mean(Y * tf.log(hypothesis) + (1 - Y) * tf.log(1 - hypothesis))
train = tf.train.GradientDescentOptimizer(learning_rate = 0.1).minimize(cost)

predicted = tf.cast(hypothesis > 0.5, dtype = tf.float32)
accuracy = tf.reduce_mean(tf.cast(tf.equal(predicted, Y), dtype = tf.float32))


# Tensorboard 사용하기!
# 1. log에 찍을 변수를 정해준다.
# 여러개의 값을 가지는 변수는 histogram, 하나의 값을 가지는 변수는 scalar
w2_hist = tf.summary.histogram("weights2", W2)
cost_summ = tf.summary.scalar("cost", cost)

with tf.Session() as sess :
	# 2. 모든 summaries들을 합쳐준다!
	merged_summary = tf.summary.merge_all()

	# 3. writer를 생성하고 그래프를 추가한다. (file로 추가되는 것임!)
	writer = tf.summary.FileWriter('./logs/xor_logs') # 저장될 경로
	writer.add_graph(sess.graph)

	sess.run(tf.global_variables_initializer())

	# 4. summary를 실행하고 writer에 summary를 추가한다
	for step in range(10001) :
		summary, _ = sess.run([merged_summary, train], feed_dict = {X : x_data, Y : y_data})
		writer.add_summary(summary, global_step = step)

	# 5. tensorboard를 실행하고 확인한다~