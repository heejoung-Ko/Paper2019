import tensorflow as tf
import numpy as np

# One hot encoding
# 벡터로 나타내기 가장 좋은 방법이다!
h = [1, 0, 0, 0]
e = [0, 1, 0, 0]
l = [0, 0, 1, 0]
o = [0, 0, 0, 1]

with tf.Session() as sess :
    hidden_size = 2 # 출력은 2개로 해주세용~
    # sequence_length : sequence(series) data를 한번에 몇개 줄거에요? = cell을 몇개 펼칠까요?
    sequence_length = 5
    # batch_size : 문자열 한번에 몇개 넣을거에요? (hello, eolll, lleel을 한번에 넣으면 3)
    batch_size = 3

    x_data = np.array([[h, e, l, l, o],
                       [e, o, l, l, l],
                       [l, l, e, e, l]], dtype = np.float32)
    print('x_data shape = ', x_data.shape)
    print('x_data = ', x_data)

    cell = tf.contrib.rnn.BasicLSTMCell(num_units = hidden_size, state_is_tuple = True)
    outputs, status = tf.nn.dynamic_rnn(cell, x_data, dtype = tf.float32)

    sess.run(tf.global_variables_initializer())
    print('outputs shape = ', outputs.eval())


