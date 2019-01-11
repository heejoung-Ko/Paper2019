import tensorflow as tf
import numpy as np

# 목표 : hi hello를 예측하는 RNN!
#        h가 맨앞에 나오면 i가 나와야하고, h의 앞에 i가 나오면 e가 나와야함


# One hot encoding
# 벡터로 나타내기 가장 좋은 방법이다!
h = [1, 0, 0, 0, 0]
i = [0, 1, 0, 0, 0]
e = [0, 0, 1, 0, 0]
l = [0, 0, 0, 1, 0]
o = [0, 0, 0, 0, 1]

# RNN parameters
hidden_size = 5      # output from the LSTM
input_dim = 5        # one-hot size
batch_size = 1       # one sentence
sequence_length = 6  # ihello == 6


idx2char = ['h', 'i', 'e', 'l', 'o'] # h = 0, i = 1, e = 2, l = 3, o = 4
x_data = [[0, 1, 0, 2, 3, 3]]        # hihell  (o는 output인 y에..)
x_one_hot = [[[1, 0, 0, 0, 0],
              [0, 1, 0, 0, 0],
              [1, 0, 0, 0, 0],
              [0, 0, 1, 0, 0],
              [0, 0, 0, 1, 0],
              [0, 0, 0, 1, 0]]]
y_data = [[1, 0, 2, 3, 3, 4]]        # ihello (맨 처음 h는 맨 처음 input값일뿐 output값이 아님)

X = tf.placeholder(tf.float32, [None, sequence_length, input_dim])
Y = tf.placeholder(tf.int32, [None, sequence_length])


# RNN model
cell = tf.contrib.rnn.BasicLSTMCell(num_units = hidden_size, state_is_tuple = True)
initial_state = cell.zero_state(batch_size, tf.float32)

outputs, _states = tf.nn.dynamic_rnn(cell, X, initial_state = initial_state,
                                     dtype = tf.float32)
weights = tf.ones([batch_size, sequence_length])

# 사실 RNN에서 나온 output을 바로 logits에 사용하는건 좋지 않지만 간단한 예제니 그냥 사용한다.
loss = tf.contrib.seq2seq.sequence_loss(logits = outputs, targets = Y, weights = weights)
train = tf.train.AdamOptimizer(learning_rate = 0.1).minimize(loss)

prediction = tf.argmax(outputs, axis = 2)


with tf.Session() as sess :
    sess.run(tf.global_variables_initializer())
    
    for i in range(2000) :
        l, _ = sess.run([loss, train], feed_dict = {X : x_one_hot, Y : y_data})
        result = sess.run(prediction, feed_dict = {X : x_one_hot})
        print(i , "loss : ", l, "prediction : ", result, "true Y : ", y_data)


        # print char using dic
        result_str = [idx2char[c] for c in np.squeeze(result)]
        print("\tPrediction str : ", ''.join(result_str))

