# CSV Analyzer Pro - Manage your spreadsheets like never before

<h1 align="center">
  <img src="https://github.com/flaminggenius/readme/blob/master/docs/img/Icon.png">
</h1>

[![Build Status](https://travis-ci.org/flaminggenius/CSVAnalyzerPro.svg?branch=master)](https://travis-ci.org/flaminggenius/CSVAnalyzerPro)
[![Code Climate](https://codeclimate.com/github/flaminggenius/CSVAnalyzerPro/badges/gpa.svg)](https://codeclimate.com/github/flaminggenius/CSVAnalyzerPro)
![contributions welcome](https://img.shields.io/badge/contributions-welcome-brightgreen.svg?style=flat)
[![Github Releases](https://img.shields.io/github/downloads/atom/atom/latest/total.svg)](https://github.com/flaminggenius/CSVAnalyzerPro)

**CSV Analyzer Pro** makes managing your spreadsheets easy and faster then ever. With speeds matching top open source editors and multiwindow support how could you not download?

<h3 align="center">
  <img src="https://github.com/flaminggenius/readme/blob/master/docs/img/overall-screenshot.png" alt="vim-devicons overall screenshot" />
</h3>

Features
--------

**CSV Analyzer Pro is built for speed and extendability**

* Extend the application through our plugin system
	* ability to override defaults and use your own styles/functionalitys
	* Make money ithin you plugins in the plugin store or publish them for free
* Fast buffering makes sure those large files open without issue
* Inline editing makes it easier for you to get down to business
* Multi-window support takes efficiency to the next level
* Manage your data easily with short commands (insert row after,insert row before,insert new column)

Quick Links
-----------

Table of Contents
-----------------

[**Quick Installation**](#quick-installation)

[**Usage**](#usage)
* [**Lightline Setup**](#lightline-setup)
* [**Powerline Setup**](#powerline-setup)
* [**Extra Configuration**](#extra-configuration)
* [**Character Mappings**](#character-mappings)

[**Features**](#detailed-features)

[**Screenshots**](#screenshots)

[**Methods**](#public-methods)

[**Developer**](#developer)
* [**API**](#api)
* [**Contributing**](#contributing)
* [**Inspiration and special thanks**](#inspiration-and-special-thanks)
* [**Todo**](#todo)
* [**License**](#license)

[**FAQ / Troubleshooting**](#faq--troubleshooting)

[**Rationale**](#rationale)

<br />

<a name="quick-installation"></a>
Quick Installation (TL;DR)
--------------------------

1. [Download the latest release](https://github.com/flaminggenius/CSVAnalyzerPro/releases)

2. Run Setup wizard in adminstration mode

3. Type `CSV Analyzer Pro` in your search bar and run the application

<br />

Developer
---------

### API

```vim
" returns the font character that represents the icon
" parameters: a:1 (filename), a:2 (isDirectory)
" both parameters optional
" by default without parameters uses buffer name
WebDevIconsGetFileTypeSymbol(...)

" returns the font character that represents
" the file format as an icon (windows, linux, mac)
WebDevIconsGetFileFormatSymbol()
```

#### API Examples

##### Simple function call

```vim
echo WebDevIconsGetFileFormatSymbol()
```

##### [vim-startify]

```vim
let entry_format = "'   ['. index .']'. repeat(' ', (3 - strlen(index)))"

if exists('*WebDevIconsGetFileTypeSymbol')  " support for vim-devicons
  let entry_format .= ". WebDevIconsGetFileTypeSymbol(entry_path) .' '.  entry_path"
else
  let entry_format .= '. entry_path'
endif
```

##### Custom status line

> Custom vim status line (not relying on vim-airline or lightline):

```vim
:set statusline=%f\ %{WebDevIconsGetFileTypeSymbol()}\ %h%w%m%r\ %=%(%l,%c%V\ %Y\ %=\ %P%)
```

Todo
----

* [ ] more filetypes to support
* [ ] [customize filetype icon colors](https://github.com/ryanoasis/vim-devicons/issues/158) (for a current solution see: [tiagofumo/vim-nerdtree-syntax-highlight](https://github.com/tiagofumo/vim-nerdtree-syntax-highlight))
* [ ] more customization options in general
* [ ] more specific FAQ and Troubleshooting help

## License

See [LICENSE](LICENSE)

FAQ / Troubleshooting
---------------------

See [FAQ][wiki-faq]

## Screenshots

See [Screenshots][wiki-screenshots]

Contributing
------------

Best ways to contribute
* Star it on GitHub - if you use it and like it please at least star it :)
* [Promote](#promotion)
* Open [issues/tickets](https://github.com/ryanoasis/vim-devicons/issues)
* Submit fixes and/or improvements with [Pull Requests](#source-code)

### Promotion

Like the project? Please support to ensure continued development going forward:
* Star this repo on [GitHub][vim-devicons-repo]
* Follow the repo on [GitHub][vim-devicons-repo]
* Vote for it on [vim.org](http://www.vim.org/scripts/script.php?script_id=5114)
* Follow me
  * [Twitter](http://twitter.com/ryanlmcintyre)
  * [GitHub](https://github.com/ryanoasis)

### Source code

Contributions and Pull Requests are welcome.

No real formal process has been setup - just stick to general good conventions for now.

Rationale
---------

After seeing the awesome theme for Atom (seti-ui) and the awesome plugins work done for NERDTree and vim-airline and wanting something like this for Vim I decided to create my first plugin.

Inspiration and special thanks
------------------------------

* [vim-airline]
* [nerdtree]
* [nerdtree-git-plugin]
* [seti-ui](https://atom.io/themes/seti-ui)
* [devicons by Theodore Vorillas](http://vorillaz.github.io/devicons)
* [benatespina development.svg.icons](https://github.com/benatespina/development.svg.icons)
* [Steve Losh](http://learnvimscriptthehardway.stevelosh.com/)
* Also thanks to the many [contributors](https://github.com/ryanoasis/vim-devicons/graphs/contributors)

License
-------

See [LICENSE](LICENSE)

<!---
Link References
-->

[Nerd Fonts]:https://github.com/ryanoasis/nerd-fonts
[Nerd Font]:https://github.com/ryanoasis/nerd-fonts
[font-installation]:https://github.com/ryanoasis/nerd-fonts#font-installation
[nerd-fonts-patcher]:https://github.com/ryanoasis/nerd-fonts#font-patcher
[patched-fonts]:https://github.com/ryanoasis/nerd-fonts/tree/master/patched-fonts
[NERDTree]:https://github.com/scrooloose/nerdtree
[vim-airline]:https://github.com/bling/vim-airline
[lightline.vim]:https://github.com/itchyny/lightline.vim
[lightline]:https://github.com/itchyny/lightline.vim
[nerdtree-git-plugin]:https://github.com/Xuyuanp/nerdtree-git-plugin
[Denite]:https://github.com/Shougo/denite.nvim
[unite]:https://github.com/Shougo/unite.vim
[unite.vim]:https://github.com/Shougo/unite.vim
[vimfiler]:https://github.com/Shougo/vimfiler.vim
[flagship]:https://github.com/tpope/vim-flagship
[CtrlP]:https://github.com/kien/ctrlp.vim
[ctrlpvim-CtrlP]:https://github.com/ctrlpvim/ctrlp.vim
[powerline]:https://github.com/powerline/powerline
[vim-startify]:https://github.com/mhinz/vim-startify
[vim-workspace]:https://github.com/bagrat/vim-workspace

[wiki-screenshots]:https://github.com/ryanoasis/vim-devicons/wiki/Screenshots
[wiki-faq]:https://github.com/ryanoasis/vim-devicons/wiki/FAQ

[vim-devicons-repo]:https://github.com/ryanoasis/vim-devicons
[vim-devicons-polls]:https://github.com/ryanoasis/vim-devicons/labels/poll
[badge-version]:http://badge.fury.io/gh/ryanoasis%2Fvim-devicons
[gitter]:https://gitter.im/ryanoasis/vim-devicons?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge "Join the chat at https://gitter.im/ryanoasis/vim-devicons (external link) ➶"

[coc]:https://github.com/ryanoasis/vim-devicons/blob/master/code_of_conduct.md "Contributor Covenant Code of Conduct"
[prs]:http://makeapullrequest.com "Make a Pull Request (external link) ➶"
[code-climate]:https://codeclimate.com/github/ryanoasis/vim-devicons "Code Climate VimDevIcons (external link) ➶"
[code-climate-issues]:https://codeclimate.com/github/ryanoasis/vim-devicons "Code Climate Issues for VimDevIcons (external link) ➶"

[img-version-badge]:https://img.shields.io/github/release/ryanoasis/vim-devicons.svg?style=flat-square
[img-gitter-badge]:https://img.shields.io/gitter/room/nwjs/nw.js.svg?style=flat-square
[img-code-climate-badge]:https://img.shields.io/codeclimate/github/ryanoasis/vim-devicons.svg?style=flat-square
[img-code-climate-issues-badge]:https://img.shields.io/codeclimate/issues/github/ryanoasis/vim-devicons.svg?style=flat-square
[img-coc-badge]: https://img.shields.io/badge/code%20of-conduct-ff69b4.svg?style=flat-square
[img-prs-badge]: https://img.shields.io/badge/PRs-welcome-brightgreen.svg?style=flat-square&logo=data%3Aimage%2Fpng%3Bbase64%2CiVBORw0KGgoAAAANSUhEUgAAACAAAAAgCAMAAABEpIrGAAACWFBMVEXXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWkrXWko2FeWCAAAAAXRSTlMAQObYZgAAAAFiS0dEAIgFHUgAAAAJcEhZcwAAI28AACNvATX8B%2FsAAAAHdElNRQfhBQMBMCLAfV85AAAAi0lEQVQ4y2NgIBYszkPmJc5ORZE9DgEJqNxmmPS%2B43AA4h5B5TIwbD5%2BHFnoKCoXYSBMBIW7CF0eAxChoPM4ARXHB4GCZEIKKA8H%2FCoWE1LAwIBfBVp6wQA1DPhVzMJMcyggCVuqxGI%2FLhWY6Z6QPKoK7HmHkDwDwxYC8gwMdSDprXiz6PHjpQxUBgCLDfI7GXNh5gAAAABJRU5ErkJggg%3D%3D
[img-visual-toc-screenshots]:https://github.com/ryanoasis/vim-devicons/wiki/screenshots/v1.0.0/branding-logo-screenshots-sm.png
[img-visual-toc-api]:https://github.com/ryanoasis/vim-devicons/wiki/screenshots/v1.0.0/branding-logo-api-sm.png
[img-visual-toc-patched-fonts]:https://raw.githubusercontent.com/ryanoasis/nerd-fonts/master/images/nerd-fonts-character-logo-md.png
[img-visual-toc-fonts-patcher]:https://raw.githubusercontent.com/ryanoasis/nerd-fonts/master/images/nerd-fonts-patcher-logo-md.png
