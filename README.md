# ⚔️ Unity 3D 서바이벌 게임

<img width="1288" height="725" alt="스크린샷 2026-08-22 오후 6 29 51" src="https://github.com/user-attachments/assets/4498206f-b91b-45ab-bceb-efd9fa3b7d99" />

`99 Nights in the Forest`를 레퍼런스로 삼아 핵심 생존 루프를 분석하고 절차적 월드 생성과 상태 머신 기반 캐릭터와 AI, 아이템·전투 시스템을 Unity로 구현한 개인 프로젝트입니다.

> 본 프로젝트는 학습 목적으로 제작했습니다.
>
> 저장소에는 저작권 및 라이선스를 고려해 외부 에셋과 일부 게임 리소스를 제외한 소스 코드만 포함되어 있습니다.

---

## 🗂️ 프로젝트 정보

| 항목 | 내용 |
|---|---|
| 개발 기간 | 2026.01 ~ 진행 중 |
| 개발 인원 | 1인 개발 |
| 개발 환경 | Unity 6000.3.10f1, C# |
| 주요 패키지 | UniTask, Newtonsoft.Json, AI Navigation |
| 버전 관리 | Git, GitHub Desktop |

## 💻 핵심 구현

### 1. JSON 기반 데이터 테이블 시스템

ScriptableObject로 관리하던 밸런스 데이터가 늘어나면서 `.asset` 파일 관리와 변경 비교가 복잡해졌습니다.
데이터를 JSON으로 분리하고 모든 테이블을 먼저 로드한 뒤, 게임 로직에서 ID로 조회하도록 처리 순서를 분리했습니다.

- `IValidatable`로 ID, 이름, 수치 범위 등 개별 데이터 검증
- `DataTable<T>`에서 중복 ID 검사
- 전체 로드 후 `DataTableValidator`에서 누락 데이터와 테이블 간 관계 검증

🔗 [DataManager](Assets/Scripts/Data/DataManager.cs) · [DataTable](Assets/Scripts/Data/DataTable.cs) · [DataTableValidator](Assets/Scripts/Data/DataTableValidator.cs)

---

### 2. 절차적 맵 생성

<img width="891" height="566" alt="스크린샷 2026-08-23 오전 11 12 57" src="https://github.com/user-attachments/assets/c8156ce7-986d-4666-8ed9-31c37c468020" />

블록 단위의 넓은 맵과 높낮이가 다른 지형을 수작업으로 배치하는 비용을 줄이기 위해 절차적 생성 방식을 적용했습니다.
하나의 Seed를 기준으로 지형, 구조물, 아이템 스팟, 적 스폰 지점, 환경 오브젝트와 NavMesh를 정해진 순서로 생성합니다.

- 동일 Seed로 같은 월드를 재현하여 테스트와 저장 상태 복원 지원
- `Seed + N` 기반의 독립된 Random 스트림으로 생성 시스템 간 영향 분리
- 이전 단계의 셀 점유 결과를 공유하여 오브젝트 중복 배치 방지

🔗 [MapGenerator](Assets/Scripts/Map/MapGenerator.cs) · [MapGrid](Assets/Scripts/Map/MapGrid.cs)

---

### 3. 게임 상태 세이브/로드 시스템

각 시스템이 복원에 필요한 상태를 `SaveData`로 정의하고 `CreateSaveData()`와 `LoadSaveData()`를 통해 변환하도록 구성했습니다.
`SaveLoadManager`는 데이터를 수집해 JSON으로 직렬화하고 불러올 때는 역직렬화한 뒤 정해진 순서로 각 시스템에 적용합니다.

```text
[저장] 각 시스템 → CreateSaveData() → SaveData → JSON 직렬화 → Save.json
[로드] Save.json → JSON 역직렬화 → SaveData → LoadSaveData() → 게임 상태 복원
```

- 생성 결과 전체 대신 고정 Seed로 기본 월드 재생성
- `ObjectRegistry`의 ID를 기준으로 런타임 객체의 변경 상태 복원
- 다시 생성되지 않는 설치 객체는 `ItemId`로 프리팹을 조회해 복원

🔗 [SaveLoadManager](Assets/Scripts/SaveLoad/SaveLoadManager.cs) · [SaveData](Assets/Scripts/SaveLoad/Data/SaveData.cs)

---

### 4. ID와 인터페이스 기반 인벤토리 시스템

구체 아이템 클래스를 직접 확인하던 타입별 조건문을 ID와 `IUpgradeable` 기반의 공통 처리 구조로 개선했습니다.
새로운 장비가 추가되어도 인벤토리의 타입별 조건문을 수정하지 않도록 확장성을 높였습니다.

- `ItemId`가 같은 아이템의 수량 합산
- 같은 `GroupId`에서는 `Level`이 높은 장비로 교체
- `IUpgradeable` 구현 여부로 업그레이드 가능한 장비 판별

🔗 [Inventory](Assets/Scripts/Eqiuippable/Inventory.cs) · [InventoryItem](Assets/Scripts/Eqiuippable/InventoryItem.cs) · [IUpgradeable](Assets/Scripts/Interfaces/IUpgradeable.cs)

---

### 5. UniTask 기반 프레임 부하 분산

한 프레임에 집중되던 맵 생성 작업과 매 프레임 반복되던 적의 경로 갱신을 UniTask로 분산했습니다.
60 FPS의 프레임 예산인 약 `16.67ms`를 고려해 반복 작업의 실행 시점을 조정했습니다.

| 개선 항목 | 개선 전 | 개선 후 | 결과 |
|---|---:|---:|---:|
| 맵 생성 구간 최대 프레임 시간 평균 | 657.28ms | 461.71ms | **29.8% 감소** |
| `SetDestination()` 호출 횟수 (적 15마리) | 822.6회/초 | 65.6회/초 | **92.0% 감소** |

- 생성 작업별 프레임당 처리 개수를 정하고 기준 도달 시 `UniTask.Yield()`로 다음 프레임에 이어서 처리
- 최초 NavMesh 생성을 비동기 갱신 방식으로 변경
- `SetDestination()`을 `Update()`에서 분리해 200ms 주기의 UniTask 루프로 갱신
- 불필요한 경로 계산을 줄이면서 평균 59 FPS 유지

맵 생성 결과는 동일한 조건으로 5회 측정한 최대 프레임 시간의 평균입니다.

🔗 [MapGenerator](Assets/Scripts/Map/MapGenerator.cs) · [NavMeshGenerator](Assets/Scripts/Map/NavMeshGenerator.cs) · [EnemyChaseState](Assets/Scripts/Enemy/EnemyStates/EnemyChaseState.cs)

## 🛠️ 트러블슈팅

### 상하 회전 시 카메라 흔들림 문제

극점 부근에서 부모 Transform의 회전과 카메라의 반복 보정이 겹치며 회전 기준이 불안정해졌습니다.
카메라를 플레이어 계층에서 분리하고 `pitch`, `yaw`를 누적한 뒤 최종 Transform을 한 번만 계산하도록 수정했습니다.

🔗 [CameraRotate](Assets/Scripts/Camera/CameraRotate.cs)

### 세이브/로드 후 아이템 Collider 비활성화 문제

자루 장착 시 자식 객체까지 탐색하면서 내부 아이템의 Collider도 함께 비활성화되는 문제를 확인했습니다.
탐색 범위를 자루 자신에게 부착된 Collider로 제한해 내부 아이템의 상태에 영향을 주지 않도록 수정했습니다.

🔗 [EquippableItem](Assets/Scripts/Eqiuippable/EquippableItem.cs)

## 📌 그 외 구현

- 상태 머신 기반 플레이어·적·신도 AI
- 낮/밤 사이클과 신도 습격 이벤트
- 아이템 수집, 장착, 전투 및 제작 시스템
- PoolData 등록 기반 오브젝트 풀
