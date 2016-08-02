from django.contrib import admin
from configurator.models import Site


# Register your models here.

class SiteAdmin(admin.ModelAdmin):
    list_display = ['name', 'description', 'hostname', 'configPort', 'mqPort']


admin.site.register(Site, SiteAdmin)
