import numpy as np
import pprint as pp
import tensorflow as tf

# 1D array
t = np.array([0, 1, 2, 3, 4, 5, 6])
pp.pprint(t)
print(t.ndim) # rank
print(t.shape) # shape
print(t[0], t[2], t[-1])
print(t[2:5], t[4:-1]) # slice [2:5] = [2, 3, 4], [4:-1] = [4, 5]
print(t[:2], t[3:])

# 2D array
t = np.array([[1, 2, 3], [4, 5, 6,], [7, 8, 9], [10, 11, 12]])
pp.pprint(t)
print(t.ndim)
print(t.shape)

with tf.Session() as sess : 
	sess.run(tf.global_variables_initializer())

	# Shape, Rank, Axis(축)
	sess = tf.Session()
	t = tf.constant([1, 2, 3, 4])
	print(tf.shape(t).eval())

	t = tf.constant([[1, 2], [3, 4]])
	print(tf.shape(t).eval())

	t = tf.constant([
						[
							[
								[1, 2, 3, 4], 
								[5, 6, 7, 8], 
								[9, 10, 11, 12]
							],
							[
								[13, 14, 15, 16], 
								[17, 18, 19, 20], 
								[21, 22, 23, 24] 
							]
						]
					])
	print(tf.shape(t).eval())

	matrix1 = tf.constant([[1, 2], [3, 4]])
	matrix2 = tf.constant([[1], [2]])
	print("Matrix 1 shape", matrix1.shape)
	print("Matrix 2 shape", matrix2.shape)
	print(tf.matmul(matrix1, matrix2).eval())
	
	# Broadcasting : rank가 다른 경우에도 행렬계산할 수 있게 해준다.
	# 잘쓰면 강력하지만 잘못쓰면 독이되므로 잘 써야한다!!
	matrix1 = tf.constant([1, 2])
	matrix2 = tf.constant(3)
	print((matrix1 + matrix2).eval())

	matrix1 = tf.constant([[1, 2]])
	matrix2 = tf.constant([3, 4])
	print((matrix1 + matrix2).eval())

	matrix1 = tf.constant([[1, 2]])
	matrix2 = tf.constant([[3],[4]])
	print((matrix1 + matrix2).eval())

	# Reduce mean : 줄여서 평균을 구한다~
	print(tf.reduce_mean([1, 2], axis = 0).eval()) # 평균인데 왜 1.5가 아님? int기 때문이다!

	x = [[1., 2.],     # --------> axis = 1, -1
		 [3., 4.]]      # |
					   # |
					   # v  axis = 0

	print(tf.reduce_mean(x).eval())
	print(tf.reduce_mean(x, axis = 0).eval()) # axis = 0 : 1,2평균, 2,4평균
	print(tf.reduce_mean(x, axis = 1).eval()) # axis = 1 : 1,2평균, 3,4평균
	print(tf.reduce_mean(x, axis = -1).eval()) # axis = -1 : 1,2평균, 3,4평균

	# Reduce sum : 합 구하기
	print(tf.reduce_sum(x).eval())
	print(tf.reduce_sum(x, axis = 0).eval())
	print(tf.reduce_sum(x, axis = 1).eval())
	print(tf.reduce_mean(tf.reduce_sum(x, axis = -1)).eval())

	# Argmax : maximum 값의 위치를 구한다
	x = [[0, 1, 2],
		 [2, 1, 0]]

	print(tf.argmax(x, axis = 0).eval()) # [0, 2]중 2, [1, 1]중 1, [2, 0]중 2가 가장 큰 값이니 [1, 0, 0]이 나옴
	print(tf.argmax(x, axis = 1).eval()) # [0, 1, 2]중에선 2, [2, 1, 0]중에서 2가 가장 크니까 [2, 0]
	print(tf.argmax(x, axis = -1).eval())

	# Reshape : 행렬의 형태를 내가 원하는 형태로 만드는 것, 가장 많이 사용한다!
	# 보통 가장 안쪽의 값은 그대로 두고 바깥쪽의 값을 수정한다
	t = np.array([[ [0, 1, 2],
				    [3, 4, 5]],

				   [[6, 7, 8],
				    [9, 10, 11]]])
	print(t.shape)
	print(tf.reshape(t, shape = [-1, 3]).eval()) # -1의 의미 : 너가 알아서 만들어줘라..
	print(tf.reshape(t, shape = [-1, 1, 3]).eval())

	# Reshape (squeeze, expand) : 줄여주고 확장해주는~
	print(tf.squeeze([[0], [1], [2]]).eval()) # [[0], [1], [2]] 짜줘라
	print(tf.expand_dims([0, 1, 2], 1).eval()) # [0, 1, 2]를 1개씩 분리해라

	# One hot : 숫자를 0 또는 1로 변환해주는 것! 내가 원하는 숫자만 핫(1)하게! 나머지는 0으로!
	print(tf.one_hot([[0], [1], [2], [0]], depth = 3).eval()) # depth가 3인 것에 0 1 2 0 만 핫하게!
	
	t = tf.one_hot([[0], [1], [2], [0]], depth = 3)
	print(tf.reshape(t, shape = [-1, 3]).eval()) # 2D로~

	# Stack
	x = [1, 4]
	y = [2, 5]
	z = [3, 6]

	print(tf.stack([x, y, z]).eval())

	print(tf.stack([x, y, z], axis = 1).eval())
	print(tf.stack([x, y, z], axis = -1).eval())
	print(tf.stack([x, y, z], axis = 0).eval())

	# Ones and Zeros like : 내가 원하는 shpae의 1 또는 0으로만 채워진 array를 얻을 수 있음
	x = [[0, 1, 2],
		 [2, 1, 0]]

	print(tf.ones_like(x).eval())
	print(tf.zeros_like(x).eval())

	# Zip : 여러개의 tensor를 한방에 묶어서 사용할 수 있게 해준다
	for x, y in zip([1, 2, 3], [4, 5, 6]) :
		print(x, y)

	for x, y, z in zip([1, 2, 3,], [4, 5, 6], [7, 8, 9]) :
		print(x, y, z)
		