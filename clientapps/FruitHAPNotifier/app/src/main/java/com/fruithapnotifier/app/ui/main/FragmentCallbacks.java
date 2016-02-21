package com.fruithapnotifier.app.ui.main;

import com.fruithapnotifier.app.common.Constants;

/**
 * Created by developer on 2/14/16.
 */
public interface FragmentCallbacks
{
    void onSectionAttached(Constants.Section section, String title);
    void updateTitle(String title);
}
