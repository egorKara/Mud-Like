#!/bin/bash
# Подавление GLib критических ошибок

export G_MESSAGES_DEBUG=none
export G_DBUS_DEBUG=none
export GLIB_DEBUG=none

# Запуск Unity с подавленными ошибками
exec "$@"
