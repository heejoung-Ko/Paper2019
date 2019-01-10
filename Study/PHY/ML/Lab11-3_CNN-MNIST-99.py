import tensorflow as tf
import numpy as np
import matplotlib.pyplot as plt
import random
from tensorflow.examples.tutorials.mnist import input_data

# Simple CNN
# Input - conv1 - pool1 - conv2 - pool2 - fully-connected layer(FC)

mnist = input_data.read_data_sets("MNIST_data/", one_hot = True) 

# Parameters
nb_classes = 10
learning_rate = 0.001
training_epochs = 15
batch_size = 100
total_batch = int(mnist.train.num_examples / batch_size)
display_step = 1

# MNIST 데이터 이미지가 28 * 28 pixel이기 때문에 28 * 28 = 784
X = tf.placeholder(tf.float32, [None, 784])
X_img = tf.reshape(X, [-1, 28, 28, 1]) # 28 x 28, 흑백
# 0 ~ 9까지 있기 때문에 class는 10
Y = tf.placeholder(tf.float32, [None, nb_classes])

# L1 ImgIn shape = (?, 28, 28, 1)
# 3 x 3, 흑백, 32개의 필터
W1 = tf.Variable(tf.random_normal([3, 3, 1, 32], stddev = 0.01))

# Conv -> (?, 28, 28, 32), Relu도 동일
_L1 = tf.nn.conv2d(X_img, W1, strides = [1, 1, 1, 1], padding = 'SAME')
_L1 = tf.nn.relu(_L1)
# Pool -> (?, 14, 14, 32)
L1 = tf.nn.max_pool(_L1, ksize = [1, 2, 2, 1], strides = [1, 2, 2, 1], padding = 'SAME')

# W1가 32장이었으니.. 32로 받고 64장 쌓겠다!
W2 = tf.Variable(tf.random_normal([3, 3, 32, 64], stddev = 0.01))

# Conv -> (?, 14, 14, 64)
_L2 = tf.nn.conv2d(L1, W2, strides = [1, 1, 1, 1], padding = 'SAME')
_L2 = tf.nn.relu(_L2)
# Pool -> (?, 7, 7, 64)
L2 = tf.nn.max_pool(_L2, ksize = [1, 2, 2, 1], strides = [1, 2, 2, 1], padding = 'SAME')
# (?, 3136) 3136
L2 = tf.reshape(L2, [-1, 7 * 7 * 64])

# FC layer 7 x 7 x 64 inputs -> 10 outputs
W3 = tf.get_variable("W3", shape = [7 * 7 * 64, 10], initializer = tf.contrib.layers.xavier_initializer())
B = tf.Variable(tf.random_normal([10]), name = 'B')

hypothesis = tf.matmul(L2, W3) + B

cost = tf.reduce_mean(tf.nn.softmax_cross_entropy_with_logits(
    logits=hypothesis, labels=Y))
optimizer = tf.train.AdamOptimizer(learning_rate = learning_rate).minimize(cost)

sess = tf.Session()
sess.run(tf.global_variables_initializer())

# Test
for epoch in range(training_epochs):
    avg_cost = 0

    for i in range(total_batch):
        batch_xs, batch_ys = mnist.train.next_batch(batch_size)
        feed_dict = {X: batch_xs, Y: batch_ys}
        c, _ = sess.run([cost, optimizer], feed_dict=feed_dict)
        avg_cost += c / total_batch

    if epoch % display_step == 0 :
	    print('Epoch : ', '%04d' % (epoch + 1), 'cost = ', '{:.9f}'.format(avg_cost))



correct_prediction = tf.equal(tf.argmax(hypothesis, 1), tf.argmax(Y, 1))
accuracy = tf.reduce_mean(tf.cast(correct_prediction, tf.float32))

print("Accuracy : ", accuracy.eval(session = sess, 
	feed_dict = {X : mnist.test.images, Y : mnist.test.labels}))


r = random.randint(0, mnist.test.num_examples - 1)
print("Label : ", sess.run(tf.arg_max(mnist.test.labels[r:r+1], 1)))
print("Prediction : ", sess.run(tf.arg_max(hypothesis, 1),
	feed_dict = {X : mnist.test.images[r:r + 1]}))

#plt.imshow(mnist.test.images[r: r+1].reshape(28, 28), cmap = 'Greys', interpolation = 'nearest')
#plt.show()
