#pragma once
#include <afxwin.h>
#include "MotorControlManager.h"

class ButtonClass : public CButton
{
public:
    ButtonClass(int axis, int direction);
    virtual ~ButtonClass();

protected:
    int axisNumber;
    int m_direction;
    MotorControlManager motorControlManager;
    afx_msg void OnLButtonDown(UINT nFlags, CPoint point);
    afx_msg void OnLButtonUp(UINT nFlags, CPoint point);
    DECLARE_MESSAGE_MAP()
};

