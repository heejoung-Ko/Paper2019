* ml-agents cmd창에서 실행하기
D:
cd D:\UnityProjects\UnityMLTest
mlagents-learn --train --slow trainer_config.yaml


* trainer_config.yami 수정하기

- gamma
미래의 보상을 현재에 평가할 때 적용하는 할인율
감마 값이 적을 수록 먼 미래의 보상을 현재에서 낮게 평가하기 때문에 즉각적으로 보여준다.
감마 값이 높으면 미래의 보상을 깍지 않고 그대로 평가하기 때문에 에이전트가 즉각적인 보상보다는 미래의 보상을 좀더 신경쓰는 경향으로 발전한다.

- lamdda
미래의 보상을 평가할 때 이전까지의 보상을 기준으로 하는 정도
람다 값이 낮으면 지금까지의 보상을 기준으로 미래의 보상을 평가하게 된다. 지금까지의 경향을 유지하는 방향으로 굳어짐.
반대로 람다 값이 높으면 이전까지의 값을 기준으로 하느느게 아니라 미래의 값을 그대로 받아들이기 때문에 높은 다양성을 가지게 된다.
너무 적으면 경향이 굳어지고, 너무 높으면 안정성이 떨어짐. 적당히 하는 것이 중요하다.

- buffer_size
모델을 갱신하기 전에 수집해야 할 step 수.
즉, 4096이라고 설정했다면 4096 step에 한번씩 모델을 갱신한다고 보면 된다.

- batch_size
몇 번의 step에서 그라디언트 디텍트를 매번 갱신할 지 결정한다.
256이라고 설정했다면, 256번 step마다 갱신하는 것이다.
버퍼사이즈의 약수여야 한다. 

- max_steps
트레이닝을 완전히 종료하는 시점
2.0e4 = 20000


* 빠르게 훈련하기
BallAcademy - Training Configuration - Time Scale: 100 -> 15

D:
cd D:\UnityProjects\UnityMLTest
mlagents-learn --train --run-id="Ball Train #" trainer_config.yaml

