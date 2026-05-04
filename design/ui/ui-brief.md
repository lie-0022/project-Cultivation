# UI 디자인 브리핑 (Claude Design 전달용)

이 문서를 그대로 Claude Design에 붙여 넣어 사용. 출력은 **Unity 6.3 UI Toolkit (UXML + USS) + 미리보기 HTML 1개** 형식으로 받는다.

---

## 1. 프로젝트 개요

| 항목 | 값 |
|---|---|
| 게임명 | Cultivation (코드명) |
| 장르 | 농장 시뮬레이션 + 가챠 + 크리처 컬렉션 + 교배 |
| 플랫폼 | PC (Windows 11) |
| 엔진 | Unity 6.3 LTS, Universal Render Pipeline (URP) |
| 카메라 | 3D 3인칭 자유 이동, 시설물 E키 상호작용 (스타듀밸리 방식) |
| 언어 | 한국어 (모든 레이블) |
| MVP 범위 | 임시 UI — 디테일 일러스트 X, 단순 형태 + 색으로 표현. 단 톤은 일관되게 |
| 해상도 | 16:9 1920×1080 기준, 1280×720까지 스케일링 가능 |

---

## 2. 게임 컨셉 / 플레이어 판타지

> "내가 만든 희귀종을 컬렉션하고 싶다."

- 식물을 길러 생명체로 만드는 **신비로운 변환의 즐거움**
- 두 종을 교배해 새로운 종을 발견하는 **실험의 쾌감**
- 농장을 직접 걸어 다니며 가꾸는 **물리적 존재감**
- 골드를 모아 더 많은 가챠/확장에 투자하는 **점진적 성장**

### 코어 루프

```
[가챠] → [씨앗 인벤토리] → [밭에 심기 (E키)] → [작물 수확]
                                                    ↓
                                              [크리처화]
                                                    ↓
                                  [사육장] ←→ [교배 시스템]
                                                    ↓
                                              [판매 → 골드]
                                                    ↓
                                             (가챠로 순환)
```

---

## 3. 디자인 톤 / 비주얼 가이드

### 무드
- **캐주얼 / cute**, 따뜻함, 음식·농장 친근감
- 레퍼런스 정서: 스타듀밸리 + 포켓몬 + 모바일 컬렉션 게임
- **금기**: 다크/grim, 픽셀아트, 미니멀 모노톤, 글래스모피즘

### 형태
- 둥근 모서리(8~16px)
- 부드러운 그림자 (soft drop shadow)
- 두꺼운 외곽선 사용 X
- 플랫 + 약간의 깊이감

### 컬러
- **글로벌 베이스**: 파스텔 베이지 `#FAF3E0`, 크림 `#FFF6E5` 류
- **시설물 컬러를 액센트로** (아래 5절 참조)
- **카드/패널**: 반투명 흰색 (#FFFFFF, alpha 0xE0~0xF0) + soft drop shadow

### 타이포
- 한국어 친화적 둥근 sans-serif
- 권장: **Pretendard**, **Spoqa Han Sans Neo**, Maplestory Light, Cafe24 류
- 본문 16~18px, 패널 타이틀 24~28px, 강조 수치(골드 등) 32~40px

### 버튼
- 둥근 사각 (8px radius)
- 호버: 살짝 들뜸 (translateY(-2px)) + 그림자 강조
- 비활성: 회색조 + 약간 투명 (opacity 0.5)
- 골드 부족 시 가챠/확장 버튼은 비활성 상태로

---

## 4. 화면 구성 (필요 패널)

### 4.1 항상 표시되는 HUD

| 위젯 | 위치 | 내용 |
|---|---|---|
| **GoldHUD** | 우상단 | 현재 골드 (예: `💰 250`) |
| **SeedInventoryHUD** | 우상단 (Gold 아래) | 씨앗 종류별 개수 (예: `🌱 당근 ×3`) |
| **CropInventoryHUD** | 우상단 (Seed 아래) | 작물 종류별 개수 |
| **InteractionPrompt** | 화면 하단 중앙 | `[E] 가챠` 등. 시설물 2~2.5m 이내 진입 시만 표시 |

> HUD는 시설물 UI가 열려 있어도 계속 보임. 단 InteractionPrompt는 UI 모드 진입 시 숨김.

### 4.2 시설물 UI 패널 (E키 진입 시 화면 중앙 모달)

#### Panel A: 가챠 (`GachaPanel`)
- **타이틀**: "씨앗 가챠"
- **내용**:
  - 1회 뽑기 비용 표시: `50 G`
  - 가챠 풀 정보 (등급/확률 표시 — Common 90% / Rare 10%)
  - **[1회 뽑기]** 버튼 (골드 부족 시 비활성)
  - 결과 표시 영역: 뽑은 씨앗 카드 1장 (씨앗 이름, 등급, 일러스트는 색 도형으로)
- **닫기**: ESC 또는 E키 또는 [닫기] 버튼

#### Panel B: 밭 플롯 (`FarmPlotPanel`)
플롯 상태에 따라 3가지 모드:
- **Empty 모드**: "어떤 씨앗을 심을까요?" + 보유 씨앗 목록 (각 씨앗마다 [심기] 버튼)
- **Growing 모드**: 진행도 바 + 남은 시간 + (취소 버튼은 MVP 제외)
- **Ready 모드**: "수확 준비 완료!" + [수확하기] 버튼
- **공통**: 우측에 [확장] 버튼 (다음 칸 비용 표시, 골드 부족 시 비활성)

#### Panel C: 사육장 (`BarnPanel`)
- **상단 탭 또는 액션 버튼**: [크리처화] / [교배 시작] / [판매]
- **슬롯 그리드**: 2×2 (확장 시 동적 추가)
  - 빈 슬롯과 채워진 슬롯 시각 구분
  - 크리처 카드: 이름, 종류 색, isBusy 표시 (회전 아이콘 등)
- **크리처화 모드**: 보유 작물 리스트 → 작물 선택 → [크리처화] 버튼
- **교배 모드**: 크리처 2마리 선택 (Busy 제외) → 예상 시간 표시 → [교배 시작]
- **활성 교배 진행 표시**: 진행도 바 + 부모 표시 + [취소]
- **확장**: [사육장 확장] 버튼 (비용 표시)

#### Panel D: 상점 (`ShopPanel`)
- 탭: [작물 판매] / [크리처 판매]
- **작물 판매**: 인벤토리 작물 목록 (각 항목 [판매] 버튼 + sellPrice)
- **크리처 판매**: 사육장 크리처 목록 (Busy 제외, 각 항목 [판매] + baseSellPrice)
- 판매 시 즉시 차감 + 골드 증가 (확인 다이얼로그 없음, MVP)

### 4.3 모든 패널 공통

- 화면 중앙 정렬, 최대 너비 800px
- 우상단 [×] 닫기 버튼
- ESC 또는 E키로도 닫힘
- 패널 열릴 때 부드러운 페이드 인 (200ms)
- 패널 뒤 배경 살짝 어둡게 (rgba(0,0,0,0.4))

---

## 5. 색상 팔레트 (씬에서 확정된 값)

이 색상들이 시설물 3D 큐브에 이미 적용됨. UI 패널은 같은 컬러를 액센트(타이틀 바, 강조 보더, 아이콘)로 사용해 일관성 유지.

| 요소 | HEX | 용도 |
|---|---|---|
| 가챠 | `#9B59B6` | 가챠 패널 액센트, GachaBuilding 큐브 |
| 상점 | `#E67E22` | 상점 패널 액센트, ShopBuilding 큐브 |
| 밭 (Empty) | `#6B4423` | 밭 패널 액센트, 플롯 기본색 |
| 밭 (Growing) | `#8B6F47` | 성장 중 진행도 바 색 |
| 밭 (Ready) | `#FFD700` | 수확 준비 강조 (반짝임 권장) |
| 사육장 | `#C0C0C0` | 사육장 패널 액센트 |
| 지면 | `#7CBB72` | (참고용, UI에는 거의 안 씀) |
| 플레이어 | `#4A90E2` | 플레이어 관련 표시 (스폰 위치 등) |

### 크리처별 컬러 (사육장/상점/가챠 결과 카드에서 사용)

| 크리처 | HEX | 표시 이름 |
|---|---|---|
| creature_carrot_horse | `#FF8C42` | 당근말 |
| creature_cabbage_cow | `#9ACD32` | 양배추소 |
| creature_tomato_chicken | `#E63946` | 토마토닭 |
| creature_salad_horse | `#2E8B57` | 샐러드말 |
| creature_ketchup_chicken | `#8B0000` | 케첩닭 |

### 등급 컬러 (씨앗 카드)

| Rarity | HEX | 라벨 |
|---|---|---|
| Common | `#9E9E9E` | 일반 |
| Rare | `#3498DB` | 희귀 |
| Epic | `#9B59B6` | 영웅 |

---

## 6. 실제 표시될 데이터 (MVP 더미)

### 씨앗 (3종)
| ID | 이름 | 성장시간 | 결과 작물 | 등급 |
|---|---|---|---|---|
| seed_carrot | 당근 씨앗 | 10초 | 당근 | Common |
| seed_cabbage | 양배추 씨앗 | 20초 | 양배추 | Common |
| seed_tomato | 토마토 씨앗 | 30초 | 토마토 | Rare |

### 작물 (3종) — sellPrice
| 이름 | 변환 결과 | sellPrice |
|---|---|---|
| 당근 | 당근말 | 5 G |
| 양배추 | 양배추소 | 8 G |
| 토마토 | 토마토닭 | 15 G |

### 크리처 (5종) — baseSellPrice
| 이름 | 가격 |
|---|---|
| 당근말 | 30 G |
| 양배추소 | 50 G |
| 토마토닭 | 100 G |
| 샐러드말 | 200 G |
| 케첩닭 | 400 G |

### 가챠
- 1회 뽑기 비용: **50 G**
- 가중치: 당근 60% / 양배추 30% / 토마토 10%

### 확장 비용 공식
`nextCost = round(baseCost × currentCount^1.5)`
- 밭 baseCost = 100 (예: 2칸 보유 → 다음 비용 283G)
- 사육장 baseCost = 200 (예: 2칸 보유 → 다음 비용 566G)

### 교배 레시피
- 당근말 + 양배추소 → 샐러드말 (60초)
- 토마토닭 + 토마토닭 → 케첩닭 (90초)
- 매칭 없음 → 부모 50/50 자기복제 (30초)

### 시작 상태
- 골드 200, 밭 2칸, 사육장 2칸, 인벤토리 비어있음

---

## 7. 출력 형식 (반드시 준수)

### 7.1 파일 구성
각 패널마다 다음 페어로:
```
GachaPanel.uxml
GachaPanel.uss
FarmPlotPanel.uxml
FarmPlotPanel.uss
BarnPanel.uxml
BarnPanel.uss
ShopPanel.uxml
ShopPanel.uss
HUD.uxml
HUD.uss
```

추가:
- `_palette.uss` — 글로벌 컬러 변수 (var(--color-...) 정의)
- `_typography.uss` — 폰트 사이즈/패밀리
- `preview.html` — 모든 패널 한 페이지에 나열한 시각 검토용 (UXML과 별개)

### 7.2 UXML 규칙
- root 요소에 `name` 속성 부여 (예: `name="GachaPanel"`)
- 외부 USS 참조: `<Style src="GachaPanel.uss" />` + `<Style src="_palette.uss" />` + `<Style src="_typography.uss" />`
- VisualElement, Button, Label, ScrollView 등 표준 컨트롤만 사용
- 한국어 텍스트는 UXML에 인라인 (예: `text="1회 뽑기"`)

### 7.3 USS 제약 (Unity 6.3 UI Toolkit)
다음만 사용:
- 레이아웃: `flex-direction`, `justify-content`, `align-items`, `flex-grow/shrink`, `width/height`, `margin/padding`, `position: absolute/relative`, `top/bottom/left/right`
- 색: `background-color`, `color`, `border-color` (HEX 또는 var)
- 보더: `border-width`, `border-radius` (단일 또는 4방향)
- 폰트: `font-size`, `unity-font-style`, `-unity-font-definition` (TTF 경로)
- 트랜스폼: `translate`, `rotate`, `scale` (개별 속성)
- 그림자 비슷한 효과는 별도 VisualElement 겹쳐서 처리
- transition: `transition-property`, `transition-duration`만

**미지원** (사용하지 말 것):
- `calc()`, `::before`/`::after`, `box-shadow`(USS는 지원하지만 변종 있음 — 필요 시 별도 element), `grid-*`, `gap`(margin으로 대체), `linear-gradient` 외 gradient

### 7.4 컬러 변수 예시 (`_palette.uss`)

```css
:root {
    --color-bg-base: #FAF3E0;
    --color-bg-cream: #FFF6E5;
    --color-panel-bg: rgba(255, 255, 255, 0.94);
    --color-text-primary: #2B2B2B;
    --color-text-muted: #777777;
    --color-accent-gacha: #9B59B6;
    --color-accent-shop: #E67E22;
    --color-accent-farm: #6B4423;
    --color-accent-barn: #C0C0C0;
    --color-state-growing: #8B6F47;
    --color-state-ready: #FFD700;
    --color-rarity-common: #9E9E9E;
    --color-rarity-rare: #3498DB;
    --color-rarity-epic: #9B59B6;
    /* 크리처 */
    --color-creature-carrot-horse: #FF8C42;
    --color-creature-cabbage-cow: #9ACD32;
    --color-creature-tomato-chicken: #E63946;
    --color-creature-salad-horse: #2E8B57;
    --color-creature-ketchup-chicken: #8B0000;
}
```

### 7.5 미리보기 HTML (`preview.html`)
- 모든 패널을 세로로 나열 (한 화면에 다 보이도록)
- 각 패널 위에 `<h2>패널명</h2>` 레이블
- UXML과 동일한 컬러/레이아웃을 CSS로 재현 (소스 코드는 별도, UXML 변환용 아님)
- 사용자가 디자인 톤 검토용으로만 사용

---

## 8. 인터랙션 요건 (참고)

### 진입/종료
- 시설물 2~2.5m 이내 진입 → 화면 하단 `[E]` 프롬프트
- E키 입력 → 해당 패널 모달 열림 + 커서 잠금 해제 + 표시
- 패널 열린 동안 플레이어 이동/카메라 회전 차단
- ESC, E키, 또는 패널 우상단 [×] → 패널 닫힘 + 커서 자동 잠금 + 이동 재개

### 골드 부족 시
- 가챠 [1회 뽑기], 밭 [확장], 사육장 [확장] 버튼: 비활성 + opacity 0.5 + 클릭 무시

### 같은 패널 동시 열기
- 어떤 패널도 동시에 1개만 열림 (다른 시설물 진입 차단됨)

### 진행도 바 (밭 Growing, 사육장 교배 진행)
- 좌→우 채움
- 색은 시설물 액센트 컬러
- 끝나기 직전(95%+) 살짝 펄스 애니메이션 권장 (선택)

---

## 9. 참고 자료 (이 프로젝트 내부 문서)

이미 확정된 결정/스펙. 디자인 시 참고하되, 본 브리핑이 우선:

- `design/decisions.md` — 모든 게임 규칙/아키텍처 결정 (진실 공급원)
- `design/gdd/00-overview.md` — 게임 전체 컨셉
- `design/gdd/01-gacha.md` ~ `08-player-interaction.md` — 시스템별 동작 명세
- `design/scene/scene-data.json` — 3D 씬 색상 (UI 액센트와 일치시킬 것)
- `design/balance/mvp-dummy-data.md` — 표시될 데이터 정확값

---

## 10. 비목표 (디자인하지 말 것)

다음은 MVP에 없으니 UI도 만들 필요 없음:
- 형질 유전 / 색상·크기 변이 표시
- 도감 UI
- 시즌/이벤트 배너
- 가챠 천장 진행도
- 가챠 10연차
- 근친 교배 패널티 표시
- 저장/불러오기 (메뉴 자체 없음)
- 사운드 / 이펙트 / 파티클
- 다중 선택 판매 UI
- 옵션/설정 메뉴
- 메인 메뉴 / 타이틀 화면

---

## 11. 산출물 체크리스트 (전달받을 때 확인용)

- [ ] `_palette.uss` (컬러 변수)
- [ ] `_typography.uss` (폰트 변수)
- [ ] `HUD.uxml` + `HUD.uss` (Gold + Seed/Crop 인벤토리 + InteractionPrompt 한 파일에)
- [ ] `GachaPanel.uxml` + `GachaPanel.uss`
- [ ] `FarmPlotPanel.uxml` + `FarmPlotPanel.uss` (Empty/Growing/Ready 3 모드 모두 포함)
- [ ] `BarnPanel.uxml` + `BarnPanel.uss` (크리처화/교배/판매 모드 포함, 활성 교배 진행 표시 포함)
- [ ] `ShopPanel.uxml` + `ShopPanel.uss` (작물 판매 / 크리처 판매 탭)
- [ ] `preview.html` (모든 패널 미리보기)
- [ ] (선택) `README.md` — 폰트 라이선스, 외부 의존성 정리

---

## 끝.

이 문서대로 출력해 주세요. 막히는 부분(USS 미지원 등) 있으면 명시하고 대안을 제안해 주세요.
