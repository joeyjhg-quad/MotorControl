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
	// TODO: ���⿡ �޽��� ó���� �ڵ带 �߰� ��/�Ǵ� �⺻���� ȣ���մϴ�.

	CButton::OnLButtonDown(nFlags, point);
	motorControlManager.jogMove(axisNumber, m_direction);
}


void ButtonClass::OnLButtonUp(UINT nFlags, CPoint point)
{
	// TODO: ���⿡ �޽��� ó���� �ڵ带 �߰� ��/�Ǵ� �⺻���� ȣ���մϴ�.

	CButton::OnLButtonUp(nFlags, point);
	motorControlManager.jogStop(axisNumber);

}