import tensorflow as tf
import random
from tensorflow.examples.tutorials.mnist import input_data

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
# 0 ~ 9까지 있기 때문에 class는 10
Y = tf.placeholder(tf.float32, [None, nb_classes])

W1 = tf.get_variable("W1", shape = [784, 512], initializer=tf.contrib.layers.xavier_initializer())
W2 = tf.get_variable("W2", shape = [512, 512], initializer = tf.contrib.layers.xavier_initializer())
W3 = tf.get_variable("W3", shape = [512, 512], initializer = tf.contrib.layers.xavier_initializer())
W4 = tf.get_variable("W4", shape = [512, 512], initializer = tf.contrib.layers.xavier_initializer())
W5 = tf.get_variable("W5", shape = [512, 10], initializer = tf.contrib.layers.xavier_initializer())

B1 = tf.Variable(tf.random_normal([512]), name = 'B1')
B2 = tf.Variable(tf.random_normal([512]), name = 'B2')
B3 = tf.Variable(tf.random_normal([512]), name = 'B3')
B4 = tf.Variable(tf.random_normal([512]), name = 'B4')
B5 = tf.Variable(tf.random_normal([10]), name = 'B5')

# Dropout
dropout_rate = tf.placeholder("float")

_L1 = tf.nn.relu(tf.matmul(X, W1) + B1)
L1 = tf.nn.dropout(_L1, dropout_rate)
_L2 = tf.nn.relu(tf.matmul(L1, W2) + B2)
L2 = tf.nn.dropout(_L2, dropout_rate)
_L3 = tf.nn.relu(tf.matmul(L2, W3) + B3)
L3 = tf.nn.dropout(_L3, dropout_rate)
_L4 = tf.nn.relu(tf.matmul(L3, W4) + B4)
L4 = tf.nn.dropout(_L4, dropout_rate)

hypothesis = tf.matmul(L4, W5) + B5

cost = tf.reduce_mean(tf.nn.softmax_cross_entropy_with_logits(
    logits=hypothesis, labels=Y))

# AdamOptimizer
optimizer = tf.train.AdamOptimizer(learning_rate = learning_rate).minimize(cost)

sess = tf.Session()
sess.run(tf.global_variables_initializer())

# Test
for epoch in range(training_epochs):
    avg_cost = 0

    for i in range(total_batch):
        batch_xs, batch_ys = mnist.train.next_batch(batch_size)
        feed_dict = {X: batch_xs, Y: batch_ys, dropout_rate: 0.7}
        c, _ = sess.run([cost, optimizer], feed_dict=feed_dict)
        avg_cost += c / total_batch

    if epoch % display_step == 0 :
	    print('Epoch : ', '%04d' % (epoch + 1), 'cost = ', '{:.9f}'.format(avg_cost))



correct_prediction = tf.equal(tf.argmax(hypothesis, 1), tf.argmax(Y, 1))
accuracy = tf.reduce_mean(tf.cast(correct_prediction, "float"))

print("Accuracy : ", accuracy.eval(session = sess, 
	feed_dict = {X : mnist.test.images, Y : mnist.test.labels, dropout_rate : 1.0}))


r = random.randint(0, mnist.test.num_examples - 1)
print("Label : ", sess.run(tf.arg_max(mnist.test.labels[r:r+1], 1)))
print("Prediction : ", sess.run(tf.arg_max(hypothesis, 1),
	feed_dict = {X : mnist.test.images[r:r + 1], dropout_rate = 1.0}))

#plt.imshow(mnist.test.images[r: r+1].reshape(28, 28), cmap = 'Greys', interpolation = 'nearest')
#plt.show()
