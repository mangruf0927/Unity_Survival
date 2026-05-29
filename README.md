# 99 Nights in the Forest 모작 프로젝트

## 프로젝트 소개

`99 Nights in the Forest`의 핵심 생존 루프를 분석하고 Unity에서 상태머신 기반 캐릭터/AI 구조와 데이터 기반 아이템, 전투 시스템으로 재구현한 프로젝트입니다.

## 프로젝트 정보

| 항목 | 내용 |
|---|---|
| 개발 기간 | 2026.01 ~ 진행 중 |
| 참여 인원 | 1인 |
| 개발 환경 | Unity 6000.3.10f1, C#, Visual Studio Code |
| 버전 관리 | GitHub Desktop |
| 라이브러리 | Newtonsoft.Json |

## 구현 내용

### 1. JSON 기반 데이터 테이블 시스템

> 플레이어, 적, 아이템, 무기, 가방 데이터를 JSON으로 분리하고  
> 로드 과정에서 Validator로 데이터 오류를 검증하도록 구현했습니다.
> 
> [코드 보러가기](https://github.com/mangruf0927/Unity_Survival/blob/main/Assets/Scripts/Data/DataTableValidator.cs)


### 2. PoolData 등록 기반 ObjectPool

> 풀링할 프리팹과 초기 생성 개수를 `PoolData`로 등록하고  
> `poolType`만으로 오브젝트를 요청하고 반환할 수 있도록 구현했습니다.
> 
> [코드 보러가기](https://github.com/mangruf0927/Unity_Survival/blob/main/Assets/Scripts/System/ObjectPool.cs)


### 3. 아이템 수집 및 인벤토리 시스템

> 아이템의 수집, 보관, 장착, 사용 흐름을 분리하고  
> 장비 교체와 탄약 보관을 인벤토리에서 관리하도록 구현했습니다.
> 
> [코드 보러가기](https://github.com/mangruf0927/Unity_Survival/blob/main/Assets/Scripts/Items/Inventory.cs)
