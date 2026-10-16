# ⚔️ Unity 3D 서바이벌 게임

<img width="1288" height="725" alt="스크린샷 2026-08-22 오후 6 29 51" src="https://github.com/user-attachments/assets/4498206f-b91b-45ab-bceb-efd9fa3b7d99" />

`99 Nights in the Forest`를 레퍼런스로 삼아 핵심 생존 루프를 분석하고 절차적 월드 생성과 상태 머신 기반 캐릭터 및 AI, 아이템/전투 시스템을 Unity로 구현한 개인 프로젝트입니다.

기능 구현뿐 아니라 **Seed 기반 월드 재현, 비동기 초기화, 데이터 기반 시스템 설계, 프레임 부하 분석 및 최적화**에 중점을 두었습니다.

🎬 [플레이 영상](https://www.youtube.com/watch?v=KTUUDHTg4jY)

> 본 프로젝트는 학습 목적으로 제작했습니다.
>
> 저장소에는 저작권 및 라이선스를 고려해 외부 에셋과 일부 게임 리소스를 제외한 소스 코드만 포함되어 있습니다.

---

## 🗂️ 프로젝트 정보

| 항목 | 내용 |
|---|---|
| 개발 기간 | 2026.01 ~ 2026.08 |
| 개발 인원 | 1인 개발 |
| 개발 환경 | Unity 6000.3.10f1, C# |
| 주요 기술 | UniTask, Addressables, Newtonsoft.Json, AI Navigation |
| 버전 관리 | Git, GitHub Desktop |

## 💻 핵심 구현

### 1. Seed 기반 절차적 맵 생성

<img width="891" height="566" alt="스크린샷 2026-08-23 오전 11 12 57" src="https://github.com/user-attachments/assets/c8156ce7-986d-4666-8ed9-31c37c468020" />

넓은 맵을 반복 생성하고 테스트 및 세이브/로드 시 동일한 월드를 재현하기 위해 Seed 기반 절차적 생성 시스템을 구현했습니다.

- 구조물, 아이템 스팟, 적 스폰, 환경 오브젝트별로 `System.Random(seed + N)`을 사용해 난수 흐름을 분리
- `MapGrid`에서 Cell의 높이와 점유 상태를 공유해 이전 생성 단계의 배치 결과 반영
- 난수 적용 전 후보 Cell을 좌표 기준으로 정렬해 동일 Seed와 생성 설정에서 배치 순서의 결정성 유지
- Perlin Noise를 활용해 인접 Cell 간 높이 차이가 완만한 단계형 지형 생성

🔗 [MapGenerator](Assets/Scripts/Map/MapGenerator.cs) · [MapGrid](Assets/Scripts/Map/MapGrid.cs)

---

### 2. UniTask 기반 프레임 부하 분산 및 최적화

동기적으로 실행되던 맵 생성 작업과 매 프레임 반복되던 AI 경로 갱신을 분석하고 실행 시점과 처리량을 조정했습니다.

**맵 생성 최적화**

- Generator 단위로 생성 작업을 분리하고 `UniTask.NextFrame()`을 통해 여러 프레임에 분산
- 60FPS 기준 프레임 예산인 16.67ms를 고려해 Generator별 처리량 조정
- 프레임 분산 이후 남아 있던 `Physics.Simulate` 스파이크를 분석
- `UpdateNavMesh()` 호출 여부와 NavMesh Job Worker 수를 비교 측정하고 8 Workers 적용

| 측정 단계 | 측정 결과 |
|---|---:|
| 동기 맵 생성 전체 소요 시간 | 평균 760.79ms |
| 프레임 분산 후 최대 프레임 시간 | 516.7ms |
| NavMesh Worker 조정 후 최대 프레임 시간 | 평균 37.90ms |

※ 위 성능 수치는 개발 중 측정한 결과입니다. 동기 생성 전체 소요 시간과 이후 최대 프레임 시간은 서로 다른 지표이므로 직접적인 감소율로 비교하지 않습니다. 최종 결과는 5회 측정한 최대 프레임 시간의 평균이며 그중 가장 큰 값은 38.75ms입니다.

**AI 경로 갱신 최적화**

- 추적 상태에서 매 프레임 호출하던 `SetDestination()`을 UniTask 기반 주기 갱신으로 변경
- 200ms 주기에서 방향 전환 지연이 발생하여 100ms 주기로 조정
- 상태 이탈 시 `CancellationToken`으로 갱신 루프 종료

| 항목 | 개선 전 | 개선 후 |
|---|---:|---:|
| SetDestination 호출 횟수 (AI 15마리) | 822.6회/초 | 121.2회/초 |

**결과: 호출 횟수 85.3% 감소**

🔗 [MapGenerator](Assets/Scripts/Map/MapGenerator.cs) · [NavMeshGenerator](Assets/Scripts/Map/NavMeshGenerator.cs) · [EnemyChaseState](Assets/Scripts/Enemy/EnemyStates/EnemyChaseState.cs)

---

### 3. JSON 기반 게임 상태 세이브/로드

월드 생성 결과 전체를 저장하는 대신 동일한 Seed로 월드를 재생성하고 객체별 변경 상태만 저장하도록 구성했습니다.

**저장 및 복원 흐름**

```text
[저장]
각 시스템 → CreateSaveData() → SaveData 수집
→ JSON 직렬화 → Save.json

[로드]
Save.json → JSON 역직렬화 → SaveData
→ 월드 재생성 → 시스템별 상태 복원
```

- 각 시스템에서 `CreateSaveData()`를 통해 복원에 필요한 데이터 생성
- `SaveLoadManager`에서 SaveData를 수집하고 JSON으로 직렬화
- Seed로 재생성되는 객체는 `InstanceId`로 검색해 변경된 상태 적용
- 런타임에 설치된 객체는 `ItemId`로 프리팹을 검색해 재생성
- 생성 객체와 설치 객체의 ID 범위를 분리해 충돌 방지

🔗 [SaveLoadManager](Assets/Scripts/SaveLoad/SaveLoadManager.cs) · [SaveData](Assets/Scripts/SaveLoad/Data/SaveData.cs)

---

### 4. Addressables 기반 비동기 리소스 로딩

기존에는 게임 화면 진입 후 리소스 로드와 맵 생성이 진행되어 월드 준비가 끝나기 전에 게임 시간이 진행되는 문제가 있었습니다.

이를 해결하기 위해 `LoadingScene`에서 초기화 순서를 제어하고 월드 생성이 완료된 후 플레이를 시작하도록 구성했습니다.

**게임 초기화 파이프라인**

```text
Data
  ↓
InGame Scene (Additive Load)
  ↓
Assets (Addressables)
  ↓
Map Generation
  ↓
NavMesh Generation
  ↓
Play
```

- `UniTask`를 활용한 비동기 초기화 순서 제어
- `CancellationToken`을 적용해 초기화 작업의 취소 흐름 관리
- Addressables 기반 맵 프리팹 비동기 로드 및 캐싱
- 맵 종료 시 리소스 핸들 해제
- 리소스와 월드 준비가 완료된 후 게임 시작

🔗 [LoadingController](Assets/Scripts/System/LoadingController.cs) · [AddressableLoader](Assets/Scripts/System/AddressableLoader.cs)

---

### 5. JSON 기반 데이터 테이블 시스템

ScriptableObject로 관리하던 밸런스 데이터가 늘어나면서 `.asset` 파일 관리와 변경 비교가 복잡해졌습니다.

데이터를 JSON으로 분리하고 전체 테이블 로드 및 검증을 완료한 뒤 게임 로직에서 ID로 조회하도록 구성했습니다.

- `IValidatable`을 통한 ID, 이름, 수치 범위 등 개별 데이터 검증
- `DataTable<T>`에서 중복 ID 검사
- `DataTableValidator`에서 누락 데이터와 테이블 간 관계 검증
- 데이터 로드와 검증, 게임 로직의 데이터 조회 책임 분리

🔗 [DataManager](Assets/Scripts/Data/DataManager.cs) · [DataTable](Assets/Scripts/Data/DataTable.cs) · [DataTableValidator](Assets/Scripts/Data/DataTableValidator.cs)

---

### 6. ID와 인터페이스 기반 인벤토리 시스템

아이템 타입별 조건문에 의존하던 처리 방식을 ID와 `IUpgradeable` 인터페이스 기반의 공통 처리 구조로 개선했습니다.

- 동일한 `ItemId`를 가진 아이템의 수량 합산
- 동일한 `GroupId`에서는 `Level`이 높은 장비로 교체
- `IUpgradeable` 구현 여부를 통해 업그레이드 가능한 장비 판별
- 새로운 장비 추가 시 인벤토리의 타입별 조건문 수정 최소화

🔗 [Inventory](Assets/Scripts/Eqiuippable/Inventory.cs) · [InventoryItem](Assets/Scripts/Eqiuippable/InventoryItem.cs) · [IUpgradeable](Assets/Scripts/Interfaces/IUpgradeable.cs)

---

## 🛠️ 트러블슈팅

### 1. 상하 회전 시 카메라 흔들림 문제

**문제**

카메라를 플레이어 계층에서 분리해 `RotateAround()`로 구현했지만 상하 회전 각도가 커질수록 화면이 흔들렸습니다.

**원인 분석**

카메라가 플레이어 머리 위를 바라보는 극점 부근에서 `forward`와 `Vector3.up`이 거의 평행해지고 반복적인 회전 보정으로 기준 축이 불안정해지는 문제를 확인했습니다.

**해결**

- 카메라를 플레이어 Transform 계층에서 분리
- 마우스 입력을 X/Y 각도로 누적
- 수직 각도를 -89°~89°로 제한
- 누적 각도를 기반으로 Quaternion과 카메라 위치를 한 번만 계산

**결과**

극점 부근에서도 카메라 흔들림 없이 안정적으로 시점을 제어하도록 개선했습니다.

🔗 [CameraRotate](Assets/Scripts/Camera/CameraRotate.cs)

---

### 2. 세이브/로드 후 아이템 Collider 비활성화 문제

**문제**

세이브 데이터를 불러온 뒤 자루에 보관했던 아이템을 꺼내도 Collider가 비활성화되어 상호작용할 수 없었습니다.

**원인 분석**

`GetComponentsInChildren<Collider>(true)`가 자루뿐 아니라 자식으로 복원된 아이템의 Collider까지 탐색하고 있었습니다.

**해결**

- Collider 탐색을 `GetComponents<Collider>()`로 변경
- 자루 자신에게 부착된 Collider만 제어하도록 책임 범위 제한

**결과**

로드 이후에도 자루에서 꺼낸 아이템의 Collider가 정상적으로 활성화되어 다시 획득할 수 있도록 수정했습니다.

🔗 [EquippableItem](Assets/Scripts/Eqiuippable/EquippableItem.cs)

---

## 📌 그 외 구현

- 상태 머신 기반 플레이어, 적, 신도 AI
- 낮/밤 사이클 및 신도 습격 이벤트
- 아이템 수집, 장착, 전투 및 제작 시스템
- PoolData 등록 기반 오브젝트 풀
