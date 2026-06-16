# CysTerra Public Repository
![Docs Actions Workflow Status](https://img.shields.io/github/actions/workflow/status/cryville/CysTerra.Public/publish_docs.yml?label=Docs)
![GitHub License](https://img.shields.io/github/license/cryville/CysTerra.Public)

CysTerra is a universal geohazard information platform that fetches information related to earthquakes, tsunamis, volcanoes, etc. from various event sources, and combines them into a unified interface with text description, map representation, and text-to-speech output.

This is the public repository of CysTerra, containing the public components in the project.

If you simply want to use the app, please visit the [project page](https://www.cryville.world/Projects/A017) instead.

If you want to learn about extension development, please refer to the [CysTerra extension development documentation](https://docs.eew.cryville.world/).

## Repository Structure
- [`Docs`](Docs) contains the source files of the [CysTerra extension development documentation](https://docs.eew.cryville.world/).
- [`Extensions`](Extensions) contains our first-party extensions which can be imported into the universal build to install additional features for the app.
- [`Interfaces`](Interfaces) points to a submodule which contains the libraries required for extension development.
- [`Sounds`](Sounds) contains the default sound effects built in the app.
