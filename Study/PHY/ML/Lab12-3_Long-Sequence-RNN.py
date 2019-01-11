import tensorflow as tf
import numpy as np

# 목표 : 긴 문장을 학습하고 예측해보자~

sentence = "if you want you"
char_set = list(set(sentence)) # set은 중복걸러준다       index -> char
char_dic = {w : i for i , w in enumerate(char_set)} # char -> index

dataX = []
dataY = []

sequence_length = 10            # 내가 자르길 원하는 글자수

for i in range(0, len(sentence) - sequence_length) :
    x_str = sentence[i : i + sequence_length]
    y_str = sentence[i + 1 : i + sequence_length + 1]

    x_data = [char_dic[c] for c in x_str]
    y_data = [char_dic[c] for c in y_str]

    dataX.append(x_data)
    dataY.append(y_data)


# RNN parameters
dic_size = len(char_set)        # RNN input size (one_hot size)
hidden_size = len(char_set)     # output size
num_classes = len(char_set)     # final output size
batch_size = len(dataX)         # one sample data, one batch


X = tf.placeholder(tf.int32, [None, sequence_length])
Y = tf.placeholder(tf.int32, [None, sequence_length])

X_one_hot = tf.one_hot(X, num_classes) # one_hot은 dimension에 변화를 주기때문에 항상 shape이 어떻게 변하는지 확인해야함!
print(X_one_hot)

def lstm_cell() :
    cell = tf.contrib.rnn.BasicLSTMCell(hidden_size, state_is_tuple = True)
    return cell

# RNN model
multi_cells = tf.contrib.rnn.MultiRNNCell([lstm_cell() for _ in range(2)], state_is_tuple = True)

outputs, _states = tf.nn.dynamic_rnn(multi_cells, X_one_hot, dtype = tf.float32)

X_for_fc = tf.reshape(outputs, [-1, hidden_size])
outputs = tf.contrib.layers.fully_connected(X_for_fc, num_classes, activation_fn = None)

outputs = tf.reshape(outputs, [batch_size, sequence_length, num_classes])

weights = tf.ones([batch_size, sequence_length])

sequence_loss = tf.contrib.seq2seq.sequence_loss(logits = outputs, targets = Y, weights = weights)
loss = tf.reduce_mean(sequence_loss)
train = tf.train.AdamOptimizer(learning_rate = 0.1).minimize(loss)

sess = tf.Session()
sess.run(tf.global_variables_initializer())

for i in range(500) :
    l, results, _ = sess.run([loss, outputs, train], feed_dict = {X : dataX, Y : dataY})

    for j, result in enumerate(results) :
        index = np.argmax(result, axis = 1)
        print(i, j, ''.join([char_set[t] for t in index]), 1)

results = sess.run(outputs, feed_dict = {X : dataX})

for j, result in enumerate(results):
    index = np.argmax(result, axis=1)
    if j is 0:  # print all for the first result to make a sentence
        print(''.join([char_set[t] for t in index]), end='')
    else:
        print(char_set[index[-1]], end='')