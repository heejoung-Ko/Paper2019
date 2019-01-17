import gym
import colorama as cr
import msvcrt as vc
import numpy as np
import random as pr
import matplotlib.pylab as plt

def rargmax(vector): 
    m = np.amax(vector)
    indices = np.nonzero(vector == m)[0]
    return pr.choice(indices)

cr.init(autoreset=True)
gym.envs.registration.register(id='FrozenLake-v3',
entry_point='gym.envs.toy_text:FrozenLakeEnv', kwargs={'map_name':'4x4', 'is_slippery':False})

env = gym.make('FrozenLake-v3')
env.render()

# 1. Q(s, a) 테이블을 만들어 0으로 초기화
Q = np.zeros([env.observation_space.n, env.action_space.n])

num_episodes = 2000 # 학습 횟수 설정

rList = [] # 얼마나 잘 학습하였는지 보기 위해 결과를 저장 

for i in range(num_episodes):
    # 2. s를 가져온다
    print("Try ", i + 1)
    print()
    state = env.reset()
    rAll = 0
    done = False
    # 3. 반복
    while not done:
        # 1) a를 취하고,
        action = rargmax(Q[state, :])   # random하게 액션을 취함
       

        # 2) r을 받고,
        # 3) s'으로 이동하고, 
        new_state, reward, done, _ = env.step(action)
        # 4) 현재 s에서의 a에 대한 테이블 업데이트
        Q[state, action] = reward + np.max(Q[new_state, :])
        # 5) s <- s'
        rAll += reward
        state = new_state

    rList.append(rAll)

print("Success rate: " + str(sum(rList)/num_episodes))
print("Final Q-Table Values")
print("LEFT DOWN RIGHT UP")
print(Q)
plt.bar(range(len(rList)), rList, color="blue")
plt.show()