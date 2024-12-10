#include "pch.h"
#include "ButtonClass.h"
BEGIN_MESSAGE_MAP(ButtonClass, CButton)
	ON_WM_LBUTTONDOWN()
	ON_WM_LBUTTONUP()
END_MESSAGE_MAP()

ButtonClass::ButtonClass(int axis, int direction)
	: axisNumber(axis), m_direction(direction) {
}

ButtonClass::~ButtonClass() {}

void ButtonClass::OnLButtonDown(UINT nFlags, CPoint point)
{
	// TODO: 여기에 메시지 처리기 코드를 추가 및/또는 기본값을 호출합니다.

	CButton::OnLButtonDown(nFlags, point);
	motorControlManager.jogMove(axisNumber, m_direction);
}


void ButtonClass::OnLButtonUp(UINT nFlags, CPoint point)
{
	// TODO: 여기에 메시지 처리기 코드를 추가 및/또는 기본값을 호출합니다.

	CButton::OnLButtonUp(nFlags, point);
	motorControlManager.jogStop(axisNumber);

}
